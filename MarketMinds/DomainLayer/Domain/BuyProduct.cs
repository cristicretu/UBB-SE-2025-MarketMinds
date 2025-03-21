using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Domain
{
    class BuyProduct : Product
    {
        public float price { get; set; }
        public BuyProduct(float price)
        {
            this.price = price;
        }
        public override float getPrice()
        {
            return price;
        }
    }
}
