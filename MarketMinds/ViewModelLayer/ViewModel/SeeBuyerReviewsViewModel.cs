using BusinessLogicLayer.Services;
using DomainLayer.Domain;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ViewModel
{
    public class SeeBuyerReviewsViewModel
    {
        public User user;
        public ReviewsService reviewsService;
        public ObservableCollection<Review> reviews {  get; set; }
        public float rating;
        public bool IsReviewsEmpty;
        public int reviewCount;

        public SeeBuyerReviewsViewModel(ReviewsService reviewsService, User user)
        {
            this.user = user;
            this.reviewsService = reviewsService;
            reviews = reviewsService.GetReviewsByBuyer(user);
            reviewCount = reviews.Count();
            if (reviewCount > 0)
            {
                rating = reviews.Average(r => r.rating);
            }
            IsReviewsEmpty = reviewCount == 0;
        }

        public void EditReview(Review review, string description, float rating)
        {
            reviewsService.EditReview(review.description, review.images, review.rating, review.sellerId,review.buyerId, description, rating);
        }

        public void DeleteReview(Review review)
        {
            reviewsService.DeleteReview(review.description, review.images, review.rating, review.sellerId, review.buyerId);
        }

        public void refreshData()
        {
            reviews = reviewsService.GetReviewsByBuyer(user);
        }

    }
}
