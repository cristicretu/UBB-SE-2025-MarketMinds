using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DomainLayer.Domain;
using MarketMinds.Services.ReviewService;

namespace BusinessLogicLayer.ViewModel
{
    public class SeeSellerReviewsViewModel
    {
        public User Seller { get; set; }
        public User Viewer { get; set; }
        public ReviewsService ReviewsService { get; set; }
        public ObservableCollection<Review> Reviews { get; set; }
        public float Rating { get; set; }
        public bool IsReviewsEmpty { get; set; }
        public int ReviewCount { get; set; }

        public SeeSellerReviewsViewModel(ReviewsService reviewsService, User seller, User viewer)
        {
            this.Seller = seller;
            this.Viewer = viewer;
            this.ReviewsService = reviewsService;
            Reviews = reviewsService.GetReviewsBySeller(seller);
            ReviewCount = Reviews.Count();
            if (ReviewCount > 0)
            {
                Rating = Reviews.Average(r => r.Rating);
            }
            IsReviewsEmpty = ReviewCount == 0;
        }

        public void MessageReviewer()
        {
            // if seller == viewer -> message with "About the product I sold you"
            // if seller != viewer -> message with "Could you tell me more about what you bought"
        }

        public void RefreshData()
        {
            Reviews = ReviewsService.GetReviewsBySeller(Seller);
            ReviewCount = Reviews.Count();
            if (ReviewCount > 0)
            {
                Rating = Reviews.Average(r => r.Rating);
            }
        }
    }
}
