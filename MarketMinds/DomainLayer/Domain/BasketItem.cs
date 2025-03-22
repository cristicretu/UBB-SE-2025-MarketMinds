using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Domain
{
    // Represents an item in the shopping basket with its associated product, quantity, and price
    public class BasketItem
    {
        public int id { get; set; }
        public Product product { get; set; }
        public int quantity { get; set; }
        public float price { get; set; }

        public BasketItem(int id, Product product, int quantity)
        {
            this.id = id;
            this.product = product;
            this.quantity = quantity;
            // this.price = product.getPrice(); getPrice() not implemented yet.
            // When getPrice() is implemented, this line will store the product's current price
            // at the time the item is added to the basket
        }

        public float getPrice()
        {
            // Calculates the total price for this basket item (unit price × quantity)
            return price * quantity;
        }
    }
}
