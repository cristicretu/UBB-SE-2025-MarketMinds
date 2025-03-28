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
        public const int MaxQuantityPerItem = 10;

        // Constructor with just the basket repository
        public BasketService(BasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
        }

        public void AddToBasket(int userId, int productId, int quantity)
        {
            if (userId <= 0) throw new ArgumentException("Invalid user ID");
            if (productId <= 0) throw new ArgumentException("Invalid product ID");

            // Apply the maximum quantity limit
            int limitedQuantity = Math.Min(quantity, MaxQuantityPerItem);

            // Get the user's basket
            Basket basket = _basketRepository.GetBasketByUser(userId);

            // Add the item with the limited quantity
            _basketRepository.AddItemToBasket(basket.Id, productId, limitedQuantity);
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

        public void UpdateProductQuantity(int userId, int productId, int quantity)
        {
            if (userId <= 0) throw new ArgumentException("Invalid user ID");
            if (productId <= 0) throw new ArgumentException("Invalid product ID");
            if (quantity < 0) throw new ArgumentException("Quantity cannot be negative");

            int limitedQuantity = Math.Min(quantity, MaxQuantityPerItem);

            try
            {
                // Get the user's basket
                Basket basket = _basketRepository.GetBasketByUser(userId);

                if (limitedQuantity == 0)
                {
                    // If quantity is zero, remove the item
                    _basketRepository.RemoveItemByProductId(basket.Id, productId);
                }
                else
                {
                    // Update the quantity
                    _basketRepository.UpdateItemQuantityByProductId(basket.Id, productId, limitedQuantity);
                }
            }
            catch (Exception ex)
            { 
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

            // Convert to uppercase for case-insensitive comparison
            string normalizedCode = code.ToUpper().Trim();

            // Dictionary of valid promo codes
            Dictionary<string, float> validCodes = new Dictionary<string, float>
            {
                { "DISCOUNT10", 0.10f },  // 10% discount
                { "WELCOME20", 0.20f },   // 20% discount 
                { "FLASH30", 0.30f },     // 30% discount
            };

            // Check if the code exists in the valid codes
            if (validCodes.TryGetValue(normalizedCode, out float discountRate))
            {
                return;
            }

            throw new InvalidOperationException("Invalid promo code");
        }

        // Add a new method to get the discount for a promo code
        public float GetPromoCodeDiscount(string code, float subtotal)
        {
            if (string.IsNullOrWhiteSpace(code)) return 0;

            // Convert to uppercase for case-insensitive comparison
            string normalizedCode = code.ToUpper().Trim();

            // Dictionary of valid promo codes
            Dictionary<string, float> validCodes = new Dictionary<string, float>
            {
                { "DISCOUNT10", 0.10f },  // 10% discount
                { "WELCOME20", 0.20f },   // 20% discount 
                { "FLASH30", 0.30f },     // 30% discount
            };

            // Check if the code exists in the valid codes
            if (validCodes.TryGetValue(normalizedCode, out float discountRate))
            {
                return subtotal * discountRate;
            }

            return 0; // No discount 
        }
    }
}