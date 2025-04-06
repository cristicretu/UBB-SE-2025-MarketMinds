using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using DomainLayer.Domain;
using MarketMinds.Services.ReviewService;
using MarketMinds.Services;

namespace ViewModelLayer.ViewModel
{
    public class ReviewCreateViewModel
    {
        public Review CurrentReview { get; set; }
        public ReviewsService ReviewsService { get; set; }
        public User Seller { get; set; }
        public User Buyer { get; set; }
        public string Description { get; set; }
        public List<Image> Images { get; set; }
        public float Rating { get; set; }
        private readonly ReviewCreationService reviewCreationService;

        public string ImagesString
        {
            get => reviewCreationService.FormatImagesString(Images);
            set => Images = reviewCreationService.ParseImagesString(value);
        }

        public ReviewCreateViewModel(ReviewsService reviewsService, User buyer, User seller)
        {
            ReviewsService = reviewsService;
            Buyer = buyer;
            Seller = seller;
            reviewCreationService = new ReviewCreationService(reviewsService);
            Images = new List<Image>();
        }

        public void SubmitReview()
        {
            Debug.WriteLine("Submitting Review");
            CurrentReview = reviewCreationService.CreateReview(Description, Images, Rating, Seller, Buyer);
        }

        public void UpdateReview()
        {
            reviewCreationService.UpdateReview(CurrentReview, Description, Rating);
        }
    }
}
