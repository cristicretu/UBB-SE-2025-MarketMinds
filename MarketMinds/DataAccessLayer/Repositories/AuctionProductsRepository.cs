using DomainLayer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer.Repositories
{
    public class AuctionProductsRepository : ProductsRepository<AuctionProduct>
    {
        private DataBaseConnection connection;

        public AuctionProductsRepository(DataBaseConnection connection)
        {
            this.connection = connection;
        }

        public override List<AuctionProduct> GetProducts()
        {
            List<AuctionProduct> auctions = new List<AuctionProduct>();

            string query = @"
        SELECT 
            ap.id,
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
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(reader.GetOrdinal("id"));
                        string description = reader.GetString(reader.GetOrdinal("description"));

                        int sellerId = reader.GetInt32(reader.GetOrdinal("seller_id"));
                        string username = reader.GetString(reader.GetOrdinal("username"));
                        string email = reader.GetString(reader.GetOrdinal("email"));
                        User seller = new User(sellerId, username, email);

                      
                        int conditionId = reader.GetInt32(reader.GetOrdinal("condition_id"));
                        string conditionTitle = reader.GetString(reader.GetOrdinal("conditionTitle"));
                        string conditionDescription =  reader.GetString(reader.GetOrdinal("conditionDescription"));
                        ProductCondition condition = new ProductCondition(conditionId, conditionTitle, conditionDescription);

                       
                        int categoryId = reader.GetInt32(reader.GetOrdinal("category_id"));
                        string categoryTitle = reader.GetString(reader.GetOrdinal("categoryTitle"));
                        string categoryDescription =  reader.GetString(reader.GetOrdinal("categoryDescription"));
                        ProductCategory category = new ProductCategory(categoryId, categoryTitle, categoryDescription);

                        DateTime start = reader.GetDateTime(reader.GetOrdinal("start_datetime"));
                        DateTime end = reader.GetDateTime(reader.GetOrdinal("end_datetime"));
                        float startingPrice = reader.GetFloat(reader.GetOrdinal("starting_price"));

                        List<ProductTag> tags = GetProductTags(id); 

                        AuctionProduct auction = new AuctionProduct(
                            id,
                            description,
                            seller,
                            condition,
                            category,
                            tags,
                            start,
                            end,
                            startingPrice
                        );

                        auctions.Add(auction);
                    }
                }
            }
            connection.CloseConnection();
            return auctions;
        }

        private List<ProductTag> GetProductTags(int productId)
        {
            var tags = new List<ProductTag>();

            string query = @"
        SELECT pt.id, pt.title
        FROM ProductTags pt
        INNER JOIN AuctionProductProductTags apt ON pt.id = apt.tag_id
        WHERE apt.product_id = @ProductId";

            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
         

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int tagId = reader.GetInt32(reader.GetOrdinal("id"));
                        string tagTitle = reader.GetString(reader.GetOrdinal("title"));

                        tags.Add(new ProductTag(tagId, tagTitle));
                    }
                }
            }

            return tags;
        }

        public override void DeleteProduct(AuctionProduct product)
        {
            string query = "DELETE FROM AuctionProducts WHERE id = @Id";

            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))

            {
                cmd.Parameters.AddWithValue("@Id", product.Id);

                cmd.ExecuteNonQuery();
            }
        }

        public override void AddProduct(AuctionProduct product)
        {
            throw new NotImplementedException();

        }


        public override void UpdateProduct(AuctionProduct product)
        {
            string query = "UPDATE AuctionProducts SET current_price = @CurrentPrice WHERE Id = @Id";
            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@CurrentPrice", product.CurrentPrice);
                cmd.Parameters.AddWithValue("@Id", product.Id);

                
                cmd.ExecuteNonQuery();
            }
        }

        public override AuctionProduct GetProductByID(int id)
        {
            throw new NotImplementedException();
          
        }
    }
}
