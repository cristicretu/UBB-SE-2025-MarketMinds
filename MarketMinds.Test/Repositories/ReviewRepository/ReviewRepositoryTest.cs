using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using MarketMinds.Repositories.ReviewRepository;
using DomainLayer.Domain;
using DataAccessLayer;
using Microsoft.Extensions.Configuration;
using MarketMinds.Test.Utils;
using Microsoft.Data.SqlClient;

namespace MarketMinds.Tests.ReviewRepositoryTest
{
    [TestFixture]
    public class ReviewRepositoryTest
    {
        private IReviewRepository reviewRepository;
        private DataBaseConnection connection;
        private IConfiguration config;
        private TestDatabaseHelper testDbHelper;

        private User testBuyer;
        private User testSeller;
        private Review testReview;

        [SetUp]
        public void Setup()
        {
            config = TestDatabaseHelper.CreateTestConfiguration();
            testDbHelper = new TestDatabaseHelper(config);

            connection = new DataBaseConnection(config);
            reviewRepository = new ReviewRepository(connection);

            testDbHelper.PrepareTestDatabase();

            // Create ReviewsPictures table if it doesn't exist
            CreateReviewsPicturesTable();

            // Initialize test users from the seed data
            testBuyer = new User(1, "alice123", "alice@example.com");
            testSeller = new User(2, "bob321", "bob@example.com");

            // Create a test review
            testReview = new Review(
                -1, // ID will be assigned when created
                "Great seller, fast shipping!",
                new List<Image>(), // Empty images list to avoid issues
                4.5f,
                testBuyer.Id,
                testSeller.Id
            );
        }

        private void CreateReviewsPicturesTable()
        {
            string createTableSql = @"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ReviewsPictures')
                BEGIN
                    CREATE TABLE ReviewsPictures (
                        id INT PRIMARY KEY IDENTITY(1,1),
                        url NVARCHAR(256) NOT NULL,
                        review_id INT NOT NULL,
                        CONSTRAINT FK_ReviewsPictures_Reviews FOREIGN KEY (review_id) REFERENCES Reviews(id) ON DELETE CASCADE
                    )
                END";

            connection.OpenConnection();
            try
            {
                using (SqlCommand cmd = new SqlCommand(createTableSql, connection.GetConnection()))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating ReviewsPictures table: {ex.Message}");
                // Continue with test even if table creation fails
            }
            finally
            {
                connection.CloseConnection();
            }
        }

        [Test]
        public void TestCreateReview_AddsNewReview()
        {
            // Act - create review and check ID was assigned
            reviewRepository.CreateReview(testReview);
            Assert.That(testReview.Id, Is.GreaterThan(0), "Review ID should be assigned after creation");

            // No need to verify retrieval here as other tests do that
        }

        [Test]
        public void TestGetAllReviewsByBuyer_ReturnsReviewsForBuyer()
        {
            // Arrange - make sure we have a review to find
            reviewRepository.CreateReview(testReview);

            // Act
            var reviews = reviewRepository.GetAllReviewsByBuyer(testBuyer);

            // Assert - should find at least our created review
            Assert.That(reviews, Is.Not.Null);
            Assert.That(reviews.Count, Is.EqualTo(1));
            Assert.That(reviews[0].BuyerId, Is.EqualTo(testBuyer.Id));
            Assert.That(reviews[0].Description, Is.EqualTo(testReview.Description));
        }

        [Test]
        public void TestGetAllReviewsBySeller_ReturnsReviewsForSeller()
        {
            // Arrange - make sure we have a review to find
            reviewRepository.CreateReview(testReview);

            // Act
            var reviews = reviewRepository.GetAllReviewsBySeller(testSeller);

            // Assert - should find at least our created review
            Assert.That(reviews, Is.Not.Null);
            Assert.That(reviews.Count, Is.EqualTo(1));
            Assert.That(reviews[0].SellerId, Is.EqualTo(testSeller.Id));
            Assert.That(reviews[0].Description, Is.EqualTo(testReview.Description));
        }

        [Test]
        public void TestEditReview_UpdatesReviewRating()
        {
            // Arrange - create a review first
            reviewRepository.CreateReview(testReview);
            int reviewId = testReview.Id;
            float newRating = 5.0f;

            // Act - update the rating
            reviewRepository.EditReview(testReview, newRating, null);

            // Assert - get fresh data and check the rating was updated
            var buyerReviews = reviewRepository.GetAllReviewsByBuyer(testBuyer);
            Assert.That(buyerReviews.Count, Is.EqualTo(1));

            var updatedReview = buyerReviews[0];
            Assert.That(updatedReview.Id, Is.EqualTo(reviewId));
            Assert.That(updatedReview.Rating, Is.EqualTo(newRating));
        }

        [Test]
        public void TestEditReview_UpdatesReviewDescription()
        {
            // Arrange - create a review first
            reviewRepository.CreateReview(testReview);
            int reviewId = testReview.Id;
            string newDescription = "Updated review description";

            // Act - update the description
            reviewRepository.EditReview(testReview, 0, newDescription);

            // Assert - get fresh data and check the description was updated
            var buyerReviews = reviewRepository.GetAllReviewsByBuyer(testBuyer);
            Assert.That(buyerReviews.Count, Is.EqualTo(1));

            var updatedReview = buyerReviews[0];
            Assert.That(updatedReview.Id, Is.EqualTo(reviewId));
            Assert.That(updatedReview.Description, Is.EqualTo(newDescription));
        }

        [Test]
        public void TestDeleteReview_RemovesReview()
        {
            // Arrange - create a review first
            reviewRepository.CreateReview(testReview);

            // Get the initial counts
            var initialBuyerReviews = reviewRepository.GetAllReviewsByBuyer(testBuyer);
            Assert.That(initialBuyerReviews.Count, Is.EqualTo(1), "Should have one review before deletion");

            // Act - delete the review
            reviewRepository.DeleteReview(testReview);

            // Assert - verify it's gone
            var buyerReviewsAfter = reviewRepository.GetAllReviewsByBuyer(testBuyer);
            var sellerReviewsAfter = reviewRepository.GetAllReviewsBySeller(testSeller);

            Assert.That(buyerReviewsAfter.Count, Is.EqualTo(0), "Buyer should have no reviews after deletion");
            Assert.That(sellerReviewsAfter.Count, Is.EqualTo(0), "Seller should have no reviews after deletion");
        }
    }
}