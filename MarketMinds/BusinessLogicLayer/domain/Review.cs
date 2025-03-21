using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Domain
{
    public class Review
    {
        public string description { get; set; } = string.Empty;
        public Image image { get; set; } = new Image("");
        public float rating {  get; set; }
        public int sellerId { get; set; }
        public int buyerId { get; set; } 
        public int productId { get; set; }

        public Review(string description, Image image, float rating, int sellerId, int buyerId, int productId)
        {
            this.description = description;
            this.image = image;
            this.rating = rating;
            this.sellerId = sellerId;
            this.buyerId = buyerId;
            this.productId = productId;
        }
    }
}
