using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.domain
{
    public class Review
    {
        public string description { get; set; } = string.Empty;
        public Image image { get; set; } = new Image();
        public float rating {  get; set; }
        public int sellerId { get; set; }
        public int buyerId { get; set; } 
        public int productId { get; set; }

        public User buyer;

        public Review(string d, Image i, float r, int s, int b, int p)
        {
            description = d;
            image = i;
            rating = r;
            sellerId = s;
            buyerId = b;
            productId = p;
        }
    }
}
