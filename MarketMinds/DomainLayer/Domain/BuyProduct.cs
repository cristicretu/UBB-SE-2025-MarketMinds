using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Domain
{
    public class BuyProduct : Product
    {
        public float price { get; set; }
        public BuyProduct(int id, User Seller, string title, string description, ProductCondition condition, ProductCategory category, List<ProductTag> tags, float price) : base(id, Seller, title, description, condition, category, tags)
        {
            this.price = price;
        }
        public override float getPrice()
        {
            return price;
        }
    }
}
