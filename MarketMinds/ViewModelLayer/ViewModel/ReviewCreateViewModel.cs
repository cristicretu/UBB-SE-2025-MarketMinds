using BusinessLogicLayer.Services;
using DomainLayer.Domain;
using System.Collections.Generic;

namespace ViewModelLayer.ViewModel
{
    public class ReviewCreateViewModel
    {
        public ReviewsService ReviewsService { get; set; }
        public User Seller { get; set; }
        public User Buyer { get; set; }
        public string Description { get; set; }
        public List<Image> Images { get; set; }
        public float Rating { get; set; }

        public ReviewCreateViewModel(ReviewsService reviewsService, User buyer)
        {
            ReviewsService = reviewsService;
            Buyer = buyer;
        }

        public void SubmitReview()
        {
            ReviewsService.AddReview(Description, Images, Rating, Seller, Buyer);
        }
    }
}
