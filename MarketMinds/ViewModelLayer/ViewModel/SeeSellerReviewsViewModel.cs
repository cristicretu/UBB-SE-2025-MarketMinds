using BusinessLogicLayer.Services;
using DomainLayer.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BusinessLogicLayer.ViewModel
{
    public class SeeSellerReviewsViewModel
    {
        public User seller;
        public User viewer;
        public ReviewsService reviewsService;
        public ObservableCollection<Review> reviews { get; set; }
        public float rating;
        public bool IsReviewsEmpty;
        public int reviewCount;

        public SeeSellerReviewsViewModel(ReviewsService reviewsService, User seller, User viewer)
        {
            this.seller = seller;
            this.viewer = viewer;
            this.reviewsService = reviewsService;
            reviews = reviewsService.GetReviewsBySeller(seller);
            reviewCount = reviews.Count();
            if (reviewCount > 0)
            {
                rating = reviews.Average(r => r.rating);
            }
            IsReviewsEmpty = reviewCount == 0;
        }

        public void messageReviewer()
        {
            // if seller == viewer -> message with "About the product I sold you" 
            // if seller != viewer -> message with "Could you tell me more about what you bought"
        }

        public void refreshData()
        {
            reviews = reviewsService.GetReviewsBySeller(seller);
        }
    }
}
