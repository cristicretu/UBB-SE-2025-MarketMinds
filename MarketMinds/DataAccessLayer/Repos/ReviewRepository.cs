using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.domain;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer.Repos
{
    internal class ReviewRepository
    {
        private DataBaseConnection connection;

        public ReviewRepository(DataBaseConnection connection)
        {
            this.connection = connection;
        }

        public List<Review> GetAllReviewsByBuyer(User buyer)
        {
            List<Review> reviews = new List<Review>();
            string query = "SELECT * FROM Reviews WHERE reviewer_id = @id";

            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@id", buyer.id);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reviews.Add(new Review(
                            reader.GetString(0),  // Assuming first column is int
                            reader.GetInt32(1),
                            reader.GetString(2),
                            reader.GetString(3),
                            reader.GetString(4),
                            reader.GetInt32(5)
                        ));
                    }
                }
            }
            connection.CloseConnection();
            return reviews;
        }

    }
}
