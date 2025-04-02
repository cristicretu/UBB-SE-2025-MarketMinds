using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using DataAccessLayer;
using DomainLayer.Domain;
using Microsoft.Data.SqlClient;

namespace MarketMinds.Repositories.ReviewRepository
{
    public class ReviewRepository : IReviewRepository
    {
        private DataBaseConnection connection;

        public ReviewRepository(DataBaseConnection connection)
        {
            this.connection = connection;
        }

        public ObservableCollection<Review> GetAllReviewsByBuyer(User buyer)
        {
            ObservableCollection<Review> reviews = new ObservableCollection<Review>();
            string query = "SELECT * FROM Reviews WHERE reviewer_id = @id";

            connection.OpenConnection();
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@id", buyer.Id);

                    ObservableCollection<int> reviewIds = new ObservableCollection<int>();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int reviewId = reader.GetInt32(0);
                            reviewIds.Add(reviewId);

                            // Add the review directly to the ObservableCollection
                            var review = new Review(
                                reviewId,
                                reader.GetString(3),
                                new List<Image>(), // Use ObservableCollection for images
                                Convert.ToSingle(reader.GetDouble(4)),
                                reader.GetInt32(2),
                                reader.GetInt32(1));
                            reviews.Add(review);
                        }
                    }

                    // Load images for each review after reading the reviews
                    foreach (var review in reviews)
                    {
                        review.images = new List<Image>(GetImages(review.id)); // Convert List<Image> to ObservableCollection<Image>
                    }
                }
            }
            finally
            {
                connection.CloseConnection();
            }

            return reviews;
        }

        public ObservableCollection<Review> GetAllReviewsBySeller(User seller)
        {
            ObservableCollection<Review> reviews = new ObservableCollection<Review>();
            string query = "SELECT * FROM Reviews WHERE seller_id = @id";

            connection.OpenConnection();
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@id", seller.Id);

                    ObservableCollection<int> reviewIds = new ObservableCollection<int>();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int reviewId = reader.GetInt32(0);
                            reviewIds.Add(reviewId);

                            // Add the review directly to the ObservableCollection
                            var review = new Review(
                                reviewId,
                                reader.GetString(3),
                                new List<Image>(), // Use ObservableCollection for images
                                Convert.ToSingle(reader.GetDouble(4)),
                                reader.GetInt32(2),
                                reader.GetInt32(1));
                            reviews.Add(review);
                        }
                    }

                    // Load images for each review after reading the reviews
                    foreach (var review in reviews)
                    {
                        review.images = new List<Image>(GetImages(review.id)); // Convert List<Image> to ObservableCollection<Image>
                    }
                }
            }
            finally
            {
                connection.CloseConnection();
            }
            return reviews;
        }

        public void CreateReview(Review review)
        {
            string insertSql = "INSERT INTO Reviews (reviewer_id, seller_id, description, rating) OUTPUT INSERTED.id VALUES (@reviewer_id, @seller_id, @description, @rating)";
            if (review.description == null)
            {
                review.description = string.Empty;
            }
            connection.OpenConnection();
            try
            {
                using (SqlCommand cmd = new SqlCommand(insertSql, connection.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@reviewer_id", review.buyerId);
                    cmd.Parameters.AddWithValue("@seller_id", review.sellerId);
                    cmd.Parameters.AddWithValue("@description", review.description);
                    cmd.Parameters.AddWithValue("@rating", review.rating);

                    review.id = (int)cmd.ExecuteScalar();
                }

                // Insert images if they exist
                if (review.images != null && review.images.Count > 0)
                {
                    string insertImageSql = "INSERT INTO ReviewsPictures (url, review_id) VALUES (@url, @review_id)";
                    foreach (var img in review.images)
                    {
                        using (SqlCommand cmd = new SqlCommand(insertImageSql, connection.GetConnection()))
                        {
                            cmd.Parameters.AddWithValue("@url", img.url);
                            cmd.Parameters.AddWithValue("@review_id", review.id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            finally
            {
                connection.CloseConnection();
            }
        }

        private List<Image> GetImages(int reviewId)
        {
            List<Image> images = new List<Image>();
            string query = "SELECT * FROM ReviewsPictures WHERE review_id = @id";

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
            if (review.id == -1)
            {
                review.id = GetReviewId(review);
            }

            if (rating != 0)
            {
                string updateQuery = "UPDATE Reviews SET rating = @rating WHERE id = @id";
                connection.OpenConnection();
                Debug.WriteLine("Connected to: " + connection.GetConnection().ServerVersion);
                using (SqlCommand cmd = new SqlCommand(updateQuery, connection.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@id", review.id);
                    cmd.Parameters.AddWithValue("@rating", rating);
                    cmd.ExecuteNonQuery();
                }
                connection.CloseConnection();
            }

            if (!string.IsNullOrEmpty(description))
            {
                string updateQuery = "UPDATE Reviews SET description = @description WHERE id = @id";
                connection.OpenConnection();
                Debug.WriteLine("Connected to: " + connection.GetConnection().ServerVersion);
                using (SqlCommand cmd = new SqlCommand(updateQuery, connection.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@id", review.id);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.ExecuteNonQuery();
                }
                connection.CloseConnection();
            }
        }
        public void DeleteReview(Review review)
        {
            if (review.id == -1)
            {
                review.id = GetReviewId(review);
            }
            connection.OpenConnection();
            DeleteImages(review.id);

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
                connection.OpenConnection();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        id = reader.GetInt32(0);
                    }
                }
            }
            connection.CloseConnection();
            return id;
        }

        private void DeleteImages(int reviewId)
        {
            string query = "DELETE FROM ReviewsPictures WHERE review_id = @id";
            using (SqlCommand cmd = new SqlCommand(query, connection.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@id", reviewId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
