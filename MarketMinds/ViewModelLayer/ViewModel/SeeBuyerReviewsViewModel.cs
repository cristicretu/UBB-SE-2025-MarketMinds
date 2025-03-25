using BusinessLogicLayer.Services;
using DomainLayer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ViewModel
{
    public class SeeBuyerReviewsViewModel
    {
        public User buyer;
        public ReviewsService reviewsService;
        public List<Review> reviews {  get; set; }

        public SeeBuyerReviewsViewModel(ReviewsService reviewsService, User buyer)
        {
            this.buyer = buyer;
            this.reviewsService = reviewsService;
            reviews = reviewsService.GetReviewsByBuyer(buyer);
        }

        public void EditReview(Review review, string description, float rating)
        {
            reviewsService.EditReview(review.description, review.images, review.rating, review.sellerId,review.buyerId, description, rating);
        }

        public void DeleteReview(Review review)
        {
            reviewsService.DeleteReview(review.description, review.images, review.rating, review.sellerId, review.buyerId);
        }
    }
}
