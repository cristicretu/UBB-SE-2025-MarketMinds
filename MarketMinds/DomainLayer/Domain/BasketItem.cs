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
        public int Id { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }

        public BasketItem(int id, Product product, int quantity)
        {
            this.Id = id;
            this.Product = product;
            this.Quantity = quantity;
            this.Price = product.getPrice();

        }

        public float GetPrice()
        {
            // Calculates the total price for this basket item (unit price × quantity)
            return Price * Quantity;
        }
    }
}
