﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.domain
{
    public class Review
    {
        // The review doesn't take into account the product for which the review has been made,
        // it can be mentioned in the description, and images
        public string description { get; set; } = string.Empty;
        public List<Image> images { get; set; }
        public float rating {  get; set; }
        public int sellerId { get; set; }
        public int buyerId { get; set; } 

        //public int productId { get; set; }

        public Review(string description, List<Image> images, float rating, int sellerId, int buyerId)
        {
            this.description = description;
            this.images = images;
            this.rating = rating;
            this.sellerId = sellerId;
            this.buyerId = buyerId;
        }
    }
}
