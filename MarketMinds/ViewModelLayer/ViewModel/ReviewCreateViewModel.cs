using BusinessLogicLayer.Services;
using DomainLayer.Domain;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
        public string ImagesString
        {
            get => Images != null ? string.Join("\n", Images.Select(img => img.url)) : "";
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Images = value.Split("\n").Select(url => new Image(url)).ToList();
                }
                else
                {
                    Images = new List<Image>();
                }
            }
        }

        public ReviewCreateViewModel(ReviewsService reviewsService, User buyer, User seller)
        {
            ReviewsService = reviewsService;
            Buyer = buyer;
            Seller = seller;
        }

        public void SubmitReview()
        {
            Debug.WriteLine("Submitting Review");
            if (Seller == null || Buyer == null || ReviewsService == null)
            {
                Debug.WriteLine("One of the required objects is null!");
                return;
            }
            ReviewsService.AddReview(Description, Images, Rating, Seller, Buyer);
        }

    }
}
