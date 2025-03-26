using System;
using System.Collections.Generic;
using System.Linq;
using DomainLayer.Domain;
using DataAccessLayer.Repositories;

namespace BusinessLogicLayer.Services
{
    public class BasketService
    {
        private readonly BasketRepository _basketRepository;

        // Constructor with just the basket repository
        public BasketService(BasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
        }

        public Basket GetBasketByUser(User user)
        {
            if (user == null || user.Id <= 0)
            {
                throw new ArgumentException("Valid user must be provided");
            }

            try
            {
                // Get the user's basket or create one if it doesn't exist
                return _basketRepository.GetBasketByUser(user.Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to retrieve user's basket", ex);
            }
        }

        public void RemoveProductFromBasket(int userId, int productId)
        {
            if (userId <= 0) throw new ArgumentException("Invalid user ID");
            if (productId <= 0) throw new ArgumentException("Invalid product ID");

            try
            {
                // Get the user's basket
                Basket basket = _basketRepository.GetBasketByUser(userId);

                // Remove the product
                _basketRepository.RemoveItemByProductId(basket.Id, productId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing product: {ex.Message}");
                throw new InvalidOperationException($"Could not remove product: {ex.Message}", ex);
            }
        }

        public void UpdateQuantity(int userId, int basketItemId, int quantity)
        {
            if (userId <= 0) throw new ArgumentException("Invalid user ID");
            if (basketItemId <= 0) throw new ArgumentException("Invalid basket item ID");
            if (quantity < 0) throw new ArgumentException("Quantity cannot be negative");

            try
            {
                // Get the user's basket
                Basket basket = _basketRepository.GetBasketByUser(userId);

                // Debug log to verify the basket and item IDs
                Console.WriteLine($"Updating item ID: {basketItemId} in basket ID: {basket.Id} to quantity: {quantity}");

                // Update the item quantity directly
                if (quantity == 0)
                {
                    //_basketRepository.RemoveItemFromBasket(basketItemId);
                }
                else
                {
                    _basketRepository.UpdateItemQuantity(basketItemId, quantity);
                }
            }
            catch (Exception ex)
            {
                // Log the exception with more detail
                Console.WriteLine($"Error updating quantity (Item ID: {basketItemId}): {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw new InvalidOperationException($"Could not update quantity: {ex.Message}", ex);
            }
        }

        public void ClearBasket(int userId)
        {
            if (userId <= 0) throw new ArgumentException("Invalid user ID");

            // Get the user's basket
            Basket basket = _basketRepository.GetBasketByUser(userId);

            // Clear the basket
            _basketRepository.ClearBasket(basket.Id);
        }

        public bool ValidateBasketBeforeCheckOut(int basketId)
        {
            if (basketId <= 0) throw new ArgumentException("Invalid basket ID");

            // Get the basket items
            List<BasketItem> items = _basketRepository.GetBasketItems(basketId);

            // Check if the basket is empty
            if (items.Count == 0)
            {
                return false;
            }

            // Check if all items have valid quantities
            foreach (BasketItem item in items)
            {
                if (item.Quantity <= 0)
                {
                    return false;
                }

                // Check if product is of valid type
                if (!item.HasValidPrice)
                {
                    return false;
                }
            }

            return true;
        }

        public void ApplyPromoCode(int basketId, string code)
        {
            if (basketId <= 0) throw new ArgumentException("Invalid basket ID");
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Promo code cannot be empty");

            // Simplified implementation - this would typically check against a database of valid codes
            if (code.ToUpper() == "DISCOUNT10")
            {
                // Apply a 10% discount
                // This would normally update the basket in the database
                // For now, just indicate success
                return;
            }

            throw new InvalidOperationException("Invalid promo code");
        }
    }
}