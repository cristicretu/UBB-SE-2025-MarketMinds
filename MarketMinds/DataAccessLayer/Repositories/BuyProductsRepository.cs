
using DomainLayer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class BuyProductsRepository : ProductsRepository
    {
        private DataBaseConnection connection;

        public BuyProductsRepository(DataBaseConnection connection)
        {
            this.connection = connection;
        }

        public void SaveBuyProduct(BuyProduct product)
        {
            BuyProduct buy = product;
            if (buy == null)
                throw new ArgumentException("Product must be of type BuyProduct.");


            string insertProductQuery = @"
            INSERT INTO BuyProducts 
            (title, description, seller_id, condition_id, category_id, price)
            VALUES 
            (@Title, @Description, @SellerId, @ConditionId, @CategoryId, @Price);
            SELECT SCOPE_IDENTITY();";

            connection.OpenConnection();

            int newProductId;
            using (SqlCommand cmd = new SqlCommand(insertProductQuery, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@Title", buy.Title);
                cmd.Parameters.AddWithValue("@Description", buy.Description);
                cmd.Parameters.AddWithValue("@SellerId", buy.Seller.id);
                cmd.Parameters.AddWithValue("@ConditionId", buy.Condition.id);
                cmd.Parameters.AddWithValue("@CategoryId", buy.Category.id);
                cmd.Parameters.AddWithValue("@Price", buy.Price);

                object result = cmd.ExecuteScalar();
                newProductId = Convert.ToInt32(result);
            }

            foreach (var tag in borrow.Tags)
            {
                string insertTagQuery = @"
            INSERT INTO BuyProductProductTags (product_id, tag_id)
            VALUES (@ProductId, @TagId)";

                using (SqlCommand cmd = new SqlCommand(insertTagQuery, connection.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@ProductId", newProductId);
                    cmd.Parameters.AddWithValue("@TagId", tag.id);
                    cmd.ExecuteNonQuery();
                }
            }

            foreach (var image in borrow.Images)
            {
                string insertImageQuery = @"
            INSERT INTO BuyProductsImages (url, product_id)
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

        public void DeleteBuyProduct(BuyProduct product)
        {
            string query = "DELETE FROM BuyProducts WHERE id = @Id";

            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@Id", product.Id);

                cmd.ExecuteNonQuery();
            }
        }

        private List<ProductTag> GetProductTags(int productId)
        {
            var tags = new List<ProductTag>();

            string query = @"
                        SELECT pt.id, pt.title
                        FROM ProductTags pt
                        INNER JOIN BuyProductProductTags bpt ON pt.id = bpt.tag_id
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

        private List<Image> GetProductImages(int productId)
        {
            var images = new List<Image>();

            string query = @"
            SELECT url
            FROM BuyProductsImages
            WHERE product_id = @ProductId";

            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int imageId = reader.GetInt32(reader.GetOrdinal("id"));
                        string url = reader.GetString(reader.GetOrdinal("url"));

                        images.Add(new Image(url));
                    }
                }
            }
            connection.CloseConnection();

            return images;
        }

        public BorrowProduct GetBuyProductByID(int productId)
        {
            BuyProduct buy = BuyProduct();

            string query = @"
                            SELECT 
                                bp.id,
                                bp.title,
                                bp.description,
                                bp.seller_id,
                                u.username,
                                u.email,
                                bp.condition_id,
                                pc.title AS conditionTitle,
                                pc.description AS conditionDescription,
                                bp.category_id,
                                cat.title AS categoryTitle,
                                cat.description AS categoryDescription,
                                bp.price
                            FROM BuyProducts bp
                            JOIN Users u ON bp.seller_id = u.id
                            JOIN ProductConditions pc ON bp.condition_id = pc.id
                            JOIN ProductCategories cat ON bp.category_id = cat.id
                            WHERE bp.id = @productID";

            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
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

                        float price = reader.GetFloat(reader.GetOrdinal("price"));

                        List<ProductTag> tags = GetProductTags(id);

                        List<Image> images = GetProductImages(id);

                        buy = BuyProduct(
                            id,
                            title,
                            description,
                            seller,
                            condition,
                            category,
                            tags,
                            images,
                            price
                        );
                }
            }
            connection.CloseConnection();
            return buy;
        }

        public override List<BuyProduct> GetAllBuyProducts()
        {
            List<BuyProduct> buys = new List<BuyProduct>();

            string query = @"
                            SELECT 
                                bp.id,
                                bp.title,
                                bp.description,
                                bp.seller_id,
                                u.username,
                                u.email,
                                bp.condition_id,
                                pc.title AS conditionTitle,
                                pc.description AS conditionDescription,
                                bp.category_id,
                                cat.title AS categoryTitle,
                                cat.description AS categoryDescription,
                                bp.price
                            FROM BuyProducts bp
                            JOIN Users u ON bp.seller_id = u.id
                            JOIN ProductConditions pc ON bp.condition_id = pc.id
                            JOIN ProductCategories cat ON bp.category_id = cat.id";

            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
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

                        float price = reader.GetFloat(reader.GetOrdinal("price"));

                        List<ProductTag> tags = GetProductTags(id);

                        List<Image> images = GetProductImages(id);

                        BuyProduct buy = new BuyProduct(
                            id,
                            title,
                            description,
                            seller,
                            condition,
                            category,
                            tags,
                            images,
                            price
                        );

                        buys.Add(buy);
                    }
                }
            }
            connection.CloseConnection();
            return buys;
        }
    }
}
