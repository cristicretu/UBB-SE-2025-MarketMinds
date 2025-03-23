using BusinessLogicLayer.Services;
using DomainLayer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ViewModel
{
    public class ReviewCreateViewModel
    {
        public ReviewsService ReviewsService;
        public User seller;
        public User buyer;
        public string description;
        public List<Image> images;
        public float rating;

        public ReviewCreateViewModel(ReviewsService reviewsService, User seller, User buyer, string description, List<Image> images, float rating) {
            ReviewsService = reviewsService;
            this.seller = seller;
            this.buyer = buyer;
            this.description = description;
            this.images = images;
            this.rating = rating;
        }

        public void submitReview()
        {
            ReviewsService.AddReview(description, images,rating, seller, buyer);
        }



    }
}
