using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Domain;
using MarketMinds.Services.ReviewService;
using Microsoft.IdentityModel.Tokens;

namespace BusinessLogicLayer.ViewModel
{
    public class SeeBuyerReviewsViewModel
    {
        public User User { get; set; }
        public ReviewsService ReviewsService { get; set; }
        public ObservableCollection<Review> Reviews { get; set; }
        public float Rating { get; set; }
        public bool IsReviewsEmpty { get; set; }
        public int ReviewCount { get; set; }

        public SeeBuyerReviewsViewModel(ReviewsService reviewsService, User user)
        {
            this.User = user;
            this.ReviewsService = reviewsService;
            Reviews = reviewsService.GetReviewsByBuyer(user);
        }

        public void EditReview(Review review, string description, float rating)
        {
            ReviewsService.EditReview(review.Description, review.Images, review.Rating, review.SellerId, review.BuyerId, description, rating);
        }

        public void DeleteReview(Review review)
        {
            ReviewsService.DeleteReview(review.Description, review.Images, review.Rating, review.SellerId, review.BuyerId);
            Reviews.Remove(review);
        }

        public void RefreshData()
        {
            Reviews = ReviewsService.GetReviewsByBuyer(User);
        }
    }
}
