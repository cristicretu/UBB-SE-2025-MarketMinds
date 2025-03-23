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
        public BuyProduct(int id, string description, string title, ProductCondition condition, ProductCategory category, List<ProductTag> tags, float price) : base(id, description, title, condition, category, tags)
        {
            this.price = price;
        }
        public override float getPrice()
        {
            return price;
        }
    }
}
