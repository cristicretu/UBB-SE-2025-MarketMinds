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

        public SeeBuyerReviewsViewModel(User buyer, ReviewsService reviewsService)
        {
            this.buyer = buyer;
            this.reviewsService = reviewsService;
        }

        // TODO: EditReview
    }
}
