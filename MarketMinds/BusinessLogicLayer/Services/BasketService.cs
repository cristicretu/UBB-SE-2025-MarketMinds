using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Domain;
using DataAccessLayer.Repositories;

namespace BusinessLogicLayer.Services
{
    public class BasketService
    {
        private BasketRepository repository;

        public BasketService(BasketRepository repository)
        {
            this.repository = repository;
        }

        public Basket GetBasketByUser(User user)
        {
            // Get the user's basket or create one if it doesn't exist
            return repository.GetBasketByUser(user.id);
        }

        public void addToBasket(int userId, int productId, int quantity)
        {
            // Get the user's basket
            Basket basket = repository.GetBasketByUser(userId);

            // Add the item to the basket
            repository.AddItemToBasket(basket.Id, productId, quantity);
        }

        public void removeFromBasket(int userId, int basketItemId)
        {
            // Get the user's basket
            Basket basket = repository.GetBasketByUser(userId);

            // Remove the item from the basket
            repository.RemoveItemFromBasket(basketItemId);
        }

        public void updateQuantity(int userId, int basketItemId, int quantity)
        {
            // Get the user's basket 
            Basket basket = repository.GetBasketByUser(userId);

            // Update the item quantity
            repository.UpdateItemQuantity(basketItemId, quantity);
        }

        public void clearBasket(int userId)
        {
            // Get the user's basket
            Basket basket = repository.GetBasketByUser(userId);

            // Clear the basket
            repository.ClearBasket(basket.Id);
        }

        public bool validateBasketBeforeCheckOut(int basketId)
        {
            // Get the basket items
            List<BasketItem> items = repository.GetBasketItems(basketId);

            // Check if the basket is empty
            if (items.Count == 0)
            {
                return false;
            }

            // Check if all items have valid quantities and are available
            foreach (BasketItem item in items)
            {
                if (item.Quantity <= 0)
                {
                    return false;
                }
            }

            return true;
        }

        public void applyPromoCode(int basketId, string code)
        {
            // This method needs to be implemented
            throw new NotImplementedException("ApplyPromoCode needs to be implemented");
        }
    }
}