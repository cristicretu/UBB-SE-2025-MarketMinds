using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.domain
{
    internal class Review
    {
        internal string _description { get; set; } = string.Empty;
        internal Image _image { get; set; } = new Image();
        internal float _rating {  get; set; }
        internal User _seller { get; set; } = new User();
        internal User _buyer { get; set; } = new User();
        internal Product _product { get; set; } = new Product();

        internal Review(string description, Image image, float rating, User seller, User buyer, Product product)
        {
            _description = description;
            _image = image;
            _rating = rating;
            _seller = seller;
            _buyer = buyer;
            _product = product;
        }
    }
}
