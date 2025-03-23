using BusinessLogicLayer.Services;
using DomainLayer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ViewModel
{
    public class SeeSellerReviewsViewModel
    {
        ReviewsService reviewsService;
        User seller;
        User viewer;

        public SeeSellerReviewsViewModel(ReviewsService reviewsService, User seller, User viewer)
        {
            this.reviewsService = reviewsService;
            this.seller = seller;
            this.viewer = viewer;
        }

        public void messageReviewer()
        {
            // if seller == viewer -> message with "About the product I sold you" 
            // if seller != viewer -> message with "Could you tell me more about what you bought"
        }
    }
}
