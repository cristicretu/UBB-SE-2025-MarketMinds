using System;
using System.Collections.Generic;
using System.Linq;
using DomainLayer.Domain;
using MarketMinds.Services.ReviewService;

namespace MarketMinds.Services
{
    public class ReviewCreationService
    {
        private readonly ReviewsService reviewsService;

        public ReviewCreationService(ReviewsService reviewsService)
        {
            this.reviewsService = reviewsService;
        }

        public Review CreateReview(string description, List<Image> images, float rating, User seller, User buyer)
        {
            if (seller == null || buyer == null || reviewsService == null)
            {
                throw new ArgumentException("One of the required objects is null!");
            }

            reviewsService.AddReview(description, images, rating, seller, buyer);
            return new Review(-1, description, images, rating, seller.Id, buyer.Id);
        }

        public void UpdateReview(Review currentReview, string newDescription, float newRating)
        {
            if (currentReview == null)
            {
                throw new ArgumentException("Current review cannot be null");
            }

            reviewsService.EditReview(
                currentReview.Description,
                currentReview.Images,
                currentReview.Rating,
                currentReview.SellerId,
                currentReview.BuyerId,
                newDescription,
                newRating);
        }

        public List<Image> ParseImagesString(string imagesString)
        {
            if (string.IsNullOrEmpty(imagesString))
            {
                return new List<Image>();
            }

            return imagesString.Split("\n")
                .Where(url => !string.IsNullOrEmpty(url))
                .Select(url => new Image(url))
                .ToList();
        }

        public string FormatImagesString(List<Image> images)
        {
            return images != null ? string.Join("\n", images.Select(img => img.Url)) : string.Empty;
        }
    }
} 