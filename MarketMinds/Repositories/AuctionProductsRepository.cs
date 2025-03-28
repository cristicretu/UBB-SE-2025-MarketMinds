
﻿using DomainLayer.Domain;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DataAccessLayer.Repositories
{
    public class AuctionProductsRepository : ProductsRepository
    {
        private DataBaseConnection connection;

        public AuctionProductsRepository(DataBaseConnection connection)
        {
            this.connection = connection;
        }

        public override List<Product> GetProducts()
        {
            List<Product> auctions = new List<Product>();
            DataTable productsTable = new DataTable();

            string mainQuery = @"
    SELECT 
        ap.id,
        ap.title,
        ap.description,
        ap.seller_id,
        u.username,
        u.email,
        ap.condition_id,
        pc.title AS conditionTitle,
        pc.description AS conditionDescription,
        ap.category_id,
        cat.title AS categoryTitle,
        cat.description AS categoryDescription,
        ap.start_datetime,
        ap.end_datetime,
        ap.starting_price
    FROM AuctionProducts ap
    JOIN Users u ON ap.seller_id = u.id
    JOIN ProductConditions pc ON ap.condition_id = pc.id
    JOIN ProductCategories cat ON ap.category_id = cat.id";

            connection.OpenConnection();

            // Load all products first
            using (SqlCommand cmd = new SqlCommand(mainQuery, connection.GetConnection()))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                productsTable.Load(reader);
            }

            // Process each product to retrieve tags and images
            foreach (DataRow row in productsTable.Rows)
            {
                int id = (int)row["id"];
                string title = (string)row["title"];
                string description = (string)row["description"];

                int sellerId = (int)row["seller_id"];
                string username = (string)row["username"];
                string email = (string)row["email"];
                User seller = new User(sellerId, username, email);

                int conditionId = (int)row["condition_id"];
                string conditionTitle = (string)row["conditionTitle"];
                string conditionDescription = (string)row["conditionDescription"];
                ProductCondition condition = new ProductCondition(conditionId, conditionTitle, conditionDescription);

                int categoryId = (int)row["category_id"];
                string categoryTitle = (string)row["categoryTitle"];
                string categoryDescription = (string)row["categoryDescription"];
                ProductCategory category = new ProductCategory(categoryId, categoryTitle, categoryDescription);

                DateTime start = (DateTime)row["start_datetime"];
                DateTime end = (DateTime)row["end_datetime"];
                double startingPriceDouble = (double)row["starting_price"];
                float startingPrice = (float)startingPriceDouble;

                // Fetch tags and images using separate queries
                List<ProductTag> tags = GetProductTags(id);
                List<Image> images = GetImages(id);

                AuctionProduct auction = new AuctionProduct(
                    id, title, description, seller, condition, category,
                    tags, images, start, end, startingPrice
                );

                auctions.Add(auction);
            }

            connection.CloseConnection();
            return auctions;
        }

        private List<Image> GetImages(int productId)
        {
            List<Image> images = new List<Image>();
            string query = "SELECT url FROM AuctionProductsImages WHERE product_id = @ProductId";

            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
                connection.OpenConnection();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string url = reader.GetString(reader.GetOrdinal("url"));
                        images.Add(new Image(url));
                    }
                }
                connection.CloseConnection();
            }
            return images;
        }

        private List<ProductTag> GetProductTags(int productId)
        {
            List<ProductTag> tags = new List<ProductTag>();
            string query = @"
    SELECT pt.id, pt.title 
    FROM ProductTags pt
    INNER JOIN AuctionProductProductTags apt ON pt.id = apt.tag_id
    WHERE apt.product_id = @ProductId";

            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
                connection.OpenConnection();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int tagId = reader.GetInt32(reader.GetOrdinal("id"));
                        string tagTitle = reader.GetString(reader.GetOrdinal("title"));
                        tags.Add(new ProductTag(tagId, tagTitle));
                    }
                }
                connection.CloseConnection();
            }
            return tags;
        }

        public override void DeleteProduct(Product product)
        {
            AuctionProduct auction = product as AuctionProduct;
            if (auction == null)
                throw new ArgumentException("Product must be of type AuctionProduct.");

            string query = "DELETE FROM AuctionProducts WHERE id = @Id";

            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))

            {
                cmd.Parameters.AddWithValue("@Id", auction.Id);

                cmd.ExecuteNonQuery();
            }
        }

        public override void AddProduct(Product product)
        {
            AuctionProduct auction = (AuctionProduct)product;
            if (auction == null)
                throw new ArgumentException("Product must be of type AuctionProduct.");


            string insertProductQuery = @"
            INSERT INTO AuctionProducts 
            (description, seller_id, condition_id, category_id, start_datetime, end_datetime, starting_price, current_price , title)
            VALUES 
            (@Description, @SellerId, @ConditionId, @CategoryId, @StartDateTime, @EndDateTime, @StartingPrice, @CurrentPrice, @Title);
            SELECT SCOPE_IDENTITY();";

            connection.OpenConnection();

            int newProductId;
            using (SqlCommand cmd = new SqlCommand(insertProductQuery, connection.GetConnection()))
            {                
                cmd.Parameters.AddWithValue("@Description", auction.Description);
                cmd.Parameters.AddWithValue("@SellerId", auction.Seller.Id);
                cmd.Parameters.AddWithValue("@ConditionId", auction.Condition.id);
                cmd.Parameters.AddWithValue("@CategoryId", auction.Category.id);
                cmd.Parameters.AddWithValue("@StartDateTime", auction.StartAuctionDate);
                cmd.Parameters.AddWithValue("@EndDateTime", auction.EndAuctionDate);
                cmd.Parameters.AddWithValue("@StartingPrice", auction.StartingPrice);
                cmd.Parameters.AddWithValue("@CurrentPrice", auction.StartingPrice);
                cmd.Parameters.AddWithValue("@Title", auction.Title);

                object result = cmd.ExecuteScalar();
                newProductId = Convert.ToInt32(result);
            }

            foreach (var tag in auction.Tags)
            {
                string insertTagQuery = @"
            INSERT INTO AuctionProductProductTags (product_id, tag_id)
            VALUES (@ProductId, @TagId)";

                using (SqlCommand cmd = new SqlCommand(insertTagQuery, connection.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@ProductId", newProductId);
                    cmd.Parameters.AddWithValue("@TagId", tag.id);
                    cmd.ExecuteNonQuery();
                }
            }

            foreach (var image in auction.Images)
            {
                string insertImageQuery = @"
            INSERT INTO AuctionProductsImages (url, product_id)
            VALUES (@Url, @ProductId)";

                using (SqlCommand cmd = new SqlCommand(insertImageQuery, connection.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@Url", image.url);
                    cmd.Parameters.AddWithValue("@ProductId", newProductId);
                    cmd.ExecuteNonQuery();
                }
            }

            connection.CloseConnection();
        }


        public override void UpdateProduct(Product product)
        {
            AuctionProduct auction = (AuctionProduct)product;
            if (auction == null)
                throw new ArgumentException("Product must be of type AuctionProduct.");

            string query = "UPDATE AuctionProducts SET current_price = @CurrentPrice WHERE Id = @Id";

            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@CurrentPrice", auction.CurrentPrice);
                cmd.Parameters.AddWithValue("@Id", auction.Id);


                cmd.ExecuteNonQuery();
            }
        }

        public override AuctionProduct GetProductByID(int APid)
        {
            AuctionProduct auction = null;

            string query = @"
            SELECT 
                ap.id,
                ap.title,
                ap.description,
                ap.seller_id,
                u.username,
                u.email,
                ap.condition_id,
                pc.title AS conditionTitle,
                pc.description AS conditionDescription,
                ap.category_id,
                cat.title AS categoryTitle,
                cat.description AS categoryDescription,
                ap.start_datetime,
                ap.end_datetime,
                ap.starting_price
            FROM AuctionProducts ap
            JOIN Users u ON ap.seller_id = u.id
            JOIN ProductConditions pc ON ap.condition_id = pc.id
            JOIN ProductCategories cat ON ap.category_id = cat.id
               WHERE ap.id = @APid";

            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@APid", APid);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                        int id = reader.GetInt32(reader.GetOrdinal("id"));
                        string title = reader.GetString(reader.GetOrdinal("title"));
                        string description = reader.GetString(reader.GetOrdinal("description"));

                        int sellerId = reader.GetInt32(reader.GetOrdinal("seller_id"));
                        string username = reader.GetString(reader.GetOrdinal("username"));
                        string email = reader.GetString(reader.GetOrdinal("email"));
                        User seller = new User(sellerId, username, email);


                        int conditionId = reader.GetInt32(reader.GetOrdinal("condition_id"));
                        string conditionTitle = reader.GetString(reader.GetOrdinal("conditionTitle"));
                        string conditionDescription = reader.GetString(reader.GetOrdinal("conditionDescription"));
                        ProductCondition condition = new ProductCondition(conditionId, conditionTitle, conditionDescription);


                        int categoryId = reader.GetInt32(reader.GetOrdinal("category_id"));
                        string categoryTitle = reader.GetString(reader.GetOrdinal("categoryTitle"));
                        string categoryDescription = reader.GetString(reader.GetOrdinal("categoryDescription"));
                        ProductCategory category = new ProductCategory(categoryId, categoryTitle, categoryDescription);

                        DateTime start = reader.GetDateTime(reader.GetOrdinal("start_datetime"));
                        DateTime end = reader.GetDateTime(reader.GetOrdinal("end_datetime"));
                        float startingPrice = reader.GetFloat(reader.GetOrdinal("starting_price"));

                    List<ProductTag> tags = GetProductTags(id);

                        List<Image> images = GetImages( id);

                        auction = new AuctionProduct(
                            id,
                            title,
                            description,
                            seller,
                            condition,
                            category,
                            tags,
                            images,
                            start,
                            end,
                            startingPrice
                        );
                    
                }
            }
            connection.CloseConnection();

            return auction;
        }

    }
}
