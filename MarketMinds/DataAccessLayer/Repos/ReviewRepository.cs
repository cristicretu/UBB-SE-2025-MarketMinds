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
            // Returns all the reviews done by the provided user 
            // input: buyer
            // output: all the buyer's reviews

            List<Review> reviews = new List<Review>();
            string query = "SELECT * FROM Reviews WHERE reviewer_id = @id";
            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                // attach to the parameter the id
                cmd.Parameters.AddWithValue("@id", buyer.id);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // creates a new Review object
                        reviews.Add(new Review(
                            reader.GetString(0),  // Assuming first column is int
                            new Image(reader.GetString(1)),
                            reader.GetFloat(2),
                            reader.GetInt32(3),
                            reader.GetInt32(4),
                            reader.GetInt32(5)
                        ));
                    }
                }
            }
            connection.CloseConnection(); // close the connection after use
            return reviews;
        }

    }
}
