using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Domain;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer.Repositories
{
    public class BasketRepository
    {
        private DataBaseConnection connection;

        public BasketRepository(DataBaseConnection connection)
        {
            this.connection = connection;
        }

        public Basket GetBasketByUser(int userId)
        {
            // Retrieves the user's basket, or creates one if it doesn't exist
            // input: userId
            // output: user's basket

            Basket basket = null;
            int basketId = -1;

            // First try to find existing basket for the user
            string query = "SELECT id FROM Baskets WHERE buyer_id = @userId";
            connection.OpenConnection();

            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    basketId = Convert.ToInt32(result);
                }
            }

            // If no basket exists, create one
            if (basketId == -1)
            {
                string insertCmd = "INSERT INTO Baskets (buyer_id) VALUES (@userId); SELECT CAST(SCOPE_IDENTITY() as int);";

                using (SqlCommand cmd = new SqlCommand(insertCmd, connection.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    basketId = (int)cmd.ExecuteScalar();
                }
            }

            // Create basket object and populate with items
            basket = new Basket(basketId);

            // Get basket items
            List<BasketItem> items = GetBasketItems(basketId);
            foreach (BasketItem item in items)
            {
                // Add each item to the basket
                basket.AddItem(item.Product, item.Quantity);
            }

            connection.CloseConnection();
            return basket;
        }

        public void RemoveItemByProductId(int basketId, int productId)
        {
            string deleteCmd = "DELETE FROM BasketItemsBuyProducts WHERE basket_id = @basketId AND product_id = @productId";

            connection.OpenConnection();
            try
            {
                using (SqlCommand cmd = new SqlCommand(deleteCmd, connection.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@basketId", basketId);
                    cmd.Parameters.AddWithValue("@productId", productId);
                    int rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                connection.CloseConnection();
            }
        }

        public List<BasketItem> GetBasketItems(int basketId)
        {
            List<BasketItem> items = new List<BasketItem>();

            // Query to get basket items with product details, conditions, and categories in one go
            string query = @"
            SELECT bi.id, bi.product_id, bi.quantity, bi.price, p.description, p.title,
                pc.id AS condition_id, pc.title AS condition_title, pc.description AS condition_description,
                pcat.id AS category_id, pcat.title AS category_title, pcat.description AS category_description,
                u.id AS seller_id, u.username AS seller_username, u.email AS seller_email
            FROM BasketItemsBuyProducts bi
            JOIN BuyProducts p ON bi.product_id = p.id
            LEFT JOIN ProductConditions pc ON p.condition_id = pc.id
            LEFT JOIN ProductCategories pcat ON p.category_id = pcat.id
            LEFT JOIN Users u ON p.seller_id = u.id
            WHERE bi.basket_id = @basketId";


            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@basketId", basketId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Inside your while(reader.Read()) loop:
                        int itemId = reader.GetInt32(0);
                        int productId = reader.GetInt32(1);
                        int quantity = reader.GetInt32(2);
                        double price = reader.GetDouble(3);
                        string description = reader.GetString(4);
                        string productTitle = reader.GetString(5);

                        // The rest of your reader code needs to be updated with new indices
                        int conditionId = reader.IsDBNull(6) ? -1 : reader.GetInt32(6);
                        string conditionTitle = reader.IsDBNull(7) ? string.Empty : reader.GetString(7);
                        string conditionDesc = reader.IsDBNull(8) ? string.Empty : reader.GetString(8);

                        int categoryId = reader.IsDBNull(9) ? -1 : reader.GetInt32(9);
                        string categoryTitle = reader.IsDBNull(10) ? string.Empty : reader.GetString(10);
                        string categoryDesc = reader.IsDBNull(11) ? string.Empty : reader.GetString(11);

                        int sellerId = reader.IsDBNull(12) ? -1 : reader.GetInt32(12);
                        string sellerUsername = reader.IsDBNull(13) ? string.Empty : reader.GetString(13);
                        string sellerEmail = reader.IsDBNull(14) ? string.Empty : reader.GetString(14);

                        // Create condition and category objects
                        ProductCondition condition = conditionId > 0 ?
                            new ProductCondition(conditionId, conditionTitle, conditionDesc) : null;

                        ProductCategory category = categoryId > 0 ?
                            new ProductCategory(categoryId, categoryTitle, categoryDesc) : null;

                        // Create the seller object
                        User seller = sellerId > 0 ?
                            new User(sellerId, sellerUsername, sellerEmail) : null;

                        // Create the product with basic information 

                        BuyProduct product = new BuyProduct(
                            productId,                   // Id
                            productTitle,                // Title
                            description,                 // Description
                            seller,                      // Seller
                            condition,                   // ProductCondition
                            category,                    // ProductCategory
                            new List<ProductTag>(),      // Tags
                            new List<Image>(),           // Images
                            (float)price                 // Price
                        );

                        // Create the basket item
                        BasketItem item = new BasketItem(itemId, product, quantity);
                        item.Price = (float)price;
                        items.Add(item);
                    }
                }
            }
            connection.CloseConnection();

            // Now load tags for each product
            foreach (BasketItem item in items)
            {
                // Get tags for this product
                string tagsQuery = @"
                    SELECT t.id, t.title
                    FROM ProductTags t
                    JOIN BuyProductProductTags pt ON t.id = pt.tag_id
                    WHERE pt.product_id = @productId";
                connection.OpenConnection();
                using (SqlCommand cmd = new SqlCommand(tagsQuery, connection.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@productId", item.Product.Id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int tagId = reader.GetInt32(0);
                            string tagTitle = reader.GetString(1);

                            item.Product.Tags.Add(new ProductTag(tagId, tagTitle));
                        }
                    }
                }
                connection.CloseConnection();
            }

            // After loading tags, now load images for each product
            foreach (BasketItem item in items)
            {
                // Get images for this product
                string imagesQuery = @"
                    SELECT id, url
                    FROM BuyProductImages
                    WHERE product_id = @productId";

                connection.OpenConnection();
                using (SqlCommand cmd = new SqlCommand(imagesQuery, connection.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@productId", item.Product.Id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string imageUrl = reader.GetString(1);

                            item.Product.Images.Add(new Image(imageUrl));
                        }
                    }
                }
                connection.CloseConnection();
            }

            return items;
        }

        public void AddItemToBasket(int basketId, int productId, int quantity)
        {
            // Add an item to the basket
            // input: basketId, productId, quantity
            // output: none

            // First, check if the item already exists in the basket
            string checkQuery = "SELECT id, quantity FROM BasketItemsBuyProducts WHERE basket_id = @basketId AND product_id = @productId";
            int existingItemId = -1;
            int existingQuantity = 0;

            connection.OpenConnection();

            using (SqlCommand cmd = new SqlCommand(checkQuery, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@basketId", basketId);
                cmd.Parameters.AddWithValue("@productId", productId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        existingItemId = reader.GetInt32(0);
                        existingQuantity = reader.GetInt32(1);
                    }
                }
            }

            // Get the current price of the product
            string priceQuery = "SELECT price FROM BuyProducts WHERE id = @productId";
            float price = 0;

            using (SqlCommand cmd = new SqlCommand(priceQuery, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@productId", productId);
                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    price = Convert.ToSingle(result);
                }
                else
                {
                    // Product doesn't exist or has no price
                    throw new Exception("Product not found or has no price");
                }
            }

            if (existingItemId != -1)
            {
                // Update existing item quantity
                string updateCmd = "UPDATE BasketItemsBuyProducts SET quantity = @quantity WHERE id = @id";

                using (SqlCommand cmd = new SqlCommand(updateCmd, connection.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@id", existingItemId);
                    cmd.Parameters.AddWithValue("@quantity", existingQuantity + quantity);
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                // Insert new item
                string insertCmd = "INSERT INTO BasketItemsBuyProducts (basket_id, product_id, quantity, price) VALUES (@basketId, @productId, @quantity, @price)";

                using (SqlCommand cmd = new SqlCommand(insertCmd, connection.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@basketId", basketId);
                    cmd.Parameters.AddWithValue("@productId", productId);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.ExecuteNonQuery();
                }
            }

            connection.CloseConnection();
        }

        public void UpdateItemQuantityByProductId(int basketId, int productId, int quantity)
        {
            string updateCmd = "UPDATE BasketItemsBuyProducts SET quantity = @quantity WHERE basket_id = @basketId AND product_id = @productId";

            connection.OpenConnection();
            try
            {
                using (SqlCommand cmd = new SqlCommand(updateCmd, connection.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@basketId", basketId);
                    cmd.Parameters.AddWithValue("@productId", productId);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                connection.CloseConnection();
            }
        }

        public void ClearBasket(int basketId)
        {
            // Remove all items from the basket
            // input: basketId
            // output: none

            string deleteCmd = "DELETE FROM BasketItemsBuyProducts WHERE basket_id = @basketId";

            connection.OpenConnection();

            using (SqlCommand cmd = new SqlCommand(deleteCmd, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@basketId", basketId);
                cmd.ExecuteNonQuery();
            }

            connection.CloseConnection();
        }
    }
}