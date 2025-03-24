using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Domain;
using Microsoft.Data.SqlClient;
using DataAccessLayer;

namespace DataAccessLayer.Repositories
{
    // TODO: Check the functions again now that id is also in review
    public class ReviewRepository
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
                cmd.Parameters.AddWithValue("@id", buyer.Id);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        List<Image> images = GetImages(reader.GetInt32(0));
                        // creates a new Review object
                        reviews.Add(new Review(
                            reader.GetInt32(0),
                            reader.GetString(3),  // description - 4th in sql table
                            images,
                            reader.GetFloat(4), //rating - 5th in sql table
                            reader.GetInt32(2), //seller
                            reader.GetInt32(1)  //buyer
                        ));
                    }
                }
            }
            connection.CloseConnection(); // close the connection after use
            return reviews;
        }

        public List<Review> GetAllReviewsBySeller(User seller)
        {
            // Returns all the reviews got by the provided user 
            // input: seller
            // output: all the seller's reviews

            List<Review> reviews = new List<Review>();
            string query = "SELECT * FROM Reviews WHERE seller_id = @id";
            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                // attach to the parameter the id
                cmd.Parameters.AddWithValue("@id", seller.Id);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        List<Image> images = GetImages(reader.GetInt32(0));
                        // creates a new Review object
                        reviews.Add(new Review(
                            reader.GetInt32(0),
                            reader.GetString(3),  // description - 4th in sql table
                            images,
                            reader.GetFloat(4), //rating - 5th in sql table
                            reader.GetInt32(2), //seller
                            reader.GetInt32(1)  //buyer
                        ));
                    }
                }
            }
            connection.CloseConnection(); // close the connection after use
            return reviews;
        }

        public void CreateReview(Review review)
        {

            // TODO: Check if the Review pair of reviewer,seller and description exists already
            string insertSql = "INSERT INTO Reviews OUTPUT INSERTED.id VALUES (@reviewer_id,@seller_id,@description,@rating)";
            connection.OpenConnection();
            using (SqlCommand cmd = new SqlCommand(insertSql, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@reviewer_id", review.buyerId);
                cmd.Parameters.AddWithValue("@seller_id", review.sellerId);
                cmd.Parameters.AddWithValue("@description", review.description);
                cmd.Parameters.AddWithValue("@rating", review.rating);

                review.id = cmd.ExecuteNonQuery();
            }

            // adding the images
            string insertImageSql = "INSERT INTO ReviewsPictures VALUES (@url, @review_id)";
            foreach (Image img in review.images)
            {
                using (SqlCommand cmd = new SqlCommand(insertImageSql, connection.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@url", img.url);
                    cmd.Parameters.AddWithValue("@review_id", review.id);

                    cmd.ExecuteNonQuery();
                }
            }

            connection.CloseConnection();
        }

        private List<Image> GetImages(int reviewId)
        {
            string query = "SELECT * FROM ReviewsPictures WHERE review_id = @id";
            List<Image> images = new List<Image>();

            // connection is already made from the function that calls this one

            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {

                cmd.Parameters.AddWithValue("@id", reviewId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        images.Add(new Image(reader.GetString(1)));
                    }
                }
            }
            return images;
        }

        public void EditReview(Review review, float rating, string description)
        {
            connection.OpenConnection();
            if (review.id == -1)
            {
                review.id = GetReviewId(review);
            }

            if (rating != 0)
            {
                string updateQuery = "UPDATE Reviews SET rating = @rating WHERE id = @id";

                using (SqlCommand cmd = new SqlCommand(updateQuery, connection.GetConnection()))
                {

                    cmd.Parameters.AddWithValue("@id", review.id);
                    cmd.Parameters.AddWithValue("@rating", rating);
                    cmd.ExecuteNonQuery();
                }
            }

            if (!string.IsNullOrEmpty(description))
            {
                string updateQuery = "UPDATE Reviews SET description = @description WHERE id = @id";

                using (SqlCommand cmd = new SqlCommand(updateQuery, connection.GetConnection()))
                {

                    cmd.Parameters.AddWithValue("@id", review.id);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.ExecuteNonQuery();
                }
            }
            connection.CloseConnection();
        }

        public void DeleteReview(Review review)
        {
            // first get the id, delete the images, then the review
            if (review.id == -1)
            {
                review.id = GetReviewId(review);
            }
            connection.OpenConnection();
            DeleteImages(review.id);

            // deleting from the Reviews Table
            string query = "DELETE FROM Reviews WHERE id = @id";
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {

                cmd.Parameters.AddWithValue("@id", review.id);
                cmd.ExecuteNonQuery();
            }
            connection.CloseConnection();
        }

        private int GetReviewId(Review review)
        {
            string query = "SELECT id FROM Reviews WHERE reviewer_id = @reviewer AND seller_id = @seller AND description = @desc";
            int id = -1;
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {

                cmd.Parameters.AddWithValue("@reviewer", review.buyerId);
                cmd.Parameters.AddWithValue("@seller", review.sellerId);
                cmd.Parameters.AddWithValue("@desc", review.description);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        id = reader.GetInt32(0);
                    }
                }
            }
            return id;
        }

        private void DeleteImages(int reviewId)
        {
            string query = "DELETE FROM ReviewsPictures WHERE id = @id";
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {

                cmd.Parameters.AddWithValue("@id", reviewId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
