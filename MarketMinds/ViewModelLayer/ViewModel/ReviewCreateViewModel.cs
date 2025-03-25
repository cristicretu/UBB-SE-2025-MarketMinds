using BusinessLogicLayer.Services;
using DomainLayer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModelLayer.ViewModel
{
    public class ReviewCreateViewModel
    {
        public ReviewsService ReviewsService;
        public User seller {  get; set; }
        public User buyer { get; set; }
        public string description { get; set; }
        public List<Image> images { get; set; } 
        public float rating { get; set; }

        // see about the warning
        public ReviewCreateViewModel(ReviewsService reviewsService, User buyer) {
            ReviewsService = reviewsService;
            this.buyer = buyer;
        }

        public void submitReview()
        {
            ReviewsService.AddReview(description, images,rating, seller, buyer);
        }



    }
}
