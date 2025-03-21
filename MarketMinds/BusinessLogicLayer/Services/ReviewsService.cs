using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Domain;

namespace BusinessLogicLayer.Services
{
    public class ReviewsService
    {
        public ReviewRepository Repository;

        public ReviewsService(ReviewRepository repository)
        {
            Repository = repository;
        }

        public List<Review> GetReviewsBySeller(User seller)
        {
            List<Review> reviews = Repository.GetAllReviewsBySeller(seller);
            // add to Review date and sort by dates
            //reviews.Sort()
            return reviews;
        }

        public List<Review> GetReviewsByBuyer(User buyer)
        {
            List<Review> reviews = Repository.GetAllReviewsByBuyer(buyer);
            // same here
            return reviews;
        }

        public void AddReview(string description, List<Image> images, float rating, User seller, User buyer)
        {
            Repository.CreateReview(new Review(-1,description, images, rating, seller.id, buyer.id));
        }

        public void EditReview(string description, List<Image> images, float rating, User seller, User buyer, string newDescription, float newRating)
        {
            // change in uml aswell
            Repository.EditReview(new Review(-1, description, images, rating, seller.id, buyer.id), newRating, newDescription);
        }

        public void DeleteReview(string description, List<Image> images, float rating, User seller, User buyer)
        {
            Repository.DeleteReview(new Review(-1, description, images, rating, seller.id, buyer.id));
        }
    }
}
