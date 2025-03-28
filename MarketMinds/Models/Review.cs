using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Domain
{
    public class Review
    {
        // The review doesn't take into account the product for which the review has been made,
        // it can be mentioned in the description, and images
        public int id { get; set; }
        public string description { get; set; } = "";
        public List<Image> images { get; set; }
        public float rating {  get; set; }
        public int sellerId { get; set; }
        public int buyerId { get; set; } 

        //public int productId { get; set; }

        public Review(int id, string description, List<Image> images, float rating, int sellerId, int buyerId)
        {
            this.id= id;
            this.description = description;
            this.images = images;
            this.rating = rating;
            this.sellerId = sellerId;
            this.buyerId = buyerId;
        }
    }
}
