using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Domain
{
    // Represents a shopping basket which contains basket items
    public class Basket
    {
        public int Id { get; set; }
        private List<BasketItem> items;

        public Basket(int id)
        {
            this.Id = id;
            this.items = new List<BasketItem>();
        }

        public float GetTotal()
        {
            // Calculates the total price of all items in the basket

            return items.Sum(item => item.GetPrice());
        }

        public void AddItem(Product product, int quantity)
        {
            // Adds a product to the basket with the specified quantity
            // If the product already exists in the basket, its quantity is increased

            // Check if the product is already in the basket
            BasketItem existingItem = items.FirstOrDefault(i => i.Product.Id == product.Id);

            if (existingItem != null)
            {
                // Update quantity of existing item
                existingItem.Quantity += quantity;
            }
            else
            {
                // Add new item
                BasketItem newItem = new(items.Count + 1, product, quantity);
                items.Add(newItem);
            }
        }

        public void RemoveItem(int basketItemId)
        {
            // Removes an item from the basket based on its ID

            BasketItem itemToRemove = items.FirstOrDefault(i => i.Id == basketItemId);
            if (itemToRemove != null)
            {
                items.Remove(itemToRemove);
            }
        }

        public void EmptyBasket()
        {
            // Removes all items from the basket

            items.Clear();
        }

        public void ApplyPromoCode(string code)
        {
            // Implementation for applying promo code logic
            // This would typically interact with a promotion service to:
            // 1. Validate the promotion code
            // 2. Check if the promotion is applicable to the current basket
            // 3. Apply the appropriate discount
        }

        public void UpdateItemQuantity(int basketItemId, int quantity)
        {
            // Updates the quantity of an item in the basket
            // If the quantity is set to 0 or less, the item is removed

            BasketItem item = items.FirstOrDefault(i => i.Id == basketItemId);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    // If quantity is zero or negative, remove the item entirely

                    RemoveItem(basketItemId);
                }
                else
                {
                    // Otherwise update to the new quantity 

                    item.Quantity = quantity;
                }
            }
        }

        public List<BasketItem> GetItems()
        {
            // Gets a copy of all items in the basket
            // Return a copy to prevent external modification
            return new List<BasketItem>(items);
        }
    }
}
