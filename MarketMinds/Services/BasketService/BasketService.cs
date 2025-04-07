using System;
using System.Collections.Generic;
using System.Linq;
using DomainLayer.Domain;
using MarketMinds.Repositories.BasketRepository;

namespace MarketMinds.Services.BasketService
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository basketRepository;
        public const int MaxQuantityPerItem = 10;
        private const int NOUSER = 0;
        private const int NOITEM = 0;
        private const int NOBASKET = 0;
        private const int NODISCOUNT = 0;
        private const int NOQUANTITY = 0;
        private const int DEFAULTQUANTITY = 1;

        // Constructor with just the basket repository
        public BasketService(IBasketRepository basketRepository_var)
        {
            basketRepository = basketRepository_var;
        }

        public void AddProductToBasket(int userId, int productId, int quantity)
        {
            if (userId <= NOUSER)
            {
                 throw new ArgumentException("Invalid user ID");
            }
            if (productId <= NOITEM)
            {
                throw new ArgumentException("Invalid product ID");
            }

            // Apply the maximum quantity limit
            int limitedQuantity = Math.Min(quantity, MaxQuantityPerItem);

            // Get the user's basket
            Basket basket = basketRepository.GetBasketByUserId(userId);

            // Add the item with the limited quantity
            basketRepository.AddItemToBasket(basket.Id, productId, limitedQuantity);
        }

        public Basket GetBasketByUser(User user)
        {
            if (user == null || user.Id <= NOUSER)
            {
                throw new ArgumentException("Valid user must be provided");
            }

            try
            {
                // Get the user's basket or create one if it doesn't exist
                return basketRepository.GetBasketByUserId(user.Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to retrieve user's basket", ex);
            }
        }

        public void RemoveProductFromBasket(int userId, int productId)
        {
            if (userId <= NOUSER)
            {
                 throw new ArgumentException("Invalid user ID");
            }
            if (productId <= NOITEM)
            {
                throw new ArgumentException("Invalid product ID");
            }

            try
            {
                // Get the user's basket
                Basket basket = basketRepository.GetBasketByUserId(userId);

                // Remove the product
                basketRepository.RemoveItemByProductId(basket.Id, productId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing product: {ex.Message}");
                throw new InvalidOperationException($"Could not remove product: {ex.Message}", ex);
            }
        }

        public void UpdateProductQuantity(int userId, int productId, int quantity)
        {
            if (userId <= NOUSER)
            {
                throw new ArgumentException("Invalid user ID");
            }
            if (productId <= NOITEM)
            {
                throw new ArgumentException("Invalid product ID");
            }
            if (quantity < NOQUANTITY)
            {
                 throw new ArgumentException("Quantity cannot be negative");
            }

            int limitedQuantity = Math.Min(quantity, MaxQuantityPerItem);

            try
            {
                // Get the user's basket
                Basket basket = basketRepository.GetBasketByUserId(userId);

                if (limitedQuantity == NOQUANTITY)
                {
                    // If quantity is zero, remove the item
                    basketRepository.RemoveItemByProductId(basket.Id, productId);
                }
                else
                {
                    // Update the quantity
                    basketRepository.UpdateItemQuantityByProductId(basket.Id, productId, limitedQuantity);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Could not update quantity: {ex.Message}", ex);
            }
        }

        public void ClearBasket(int userId)
        {
            if (userId <= NOUSER)
            {
                throw new ArgumentException("Invalid user ID");
            }

            // Get the user's basket
            Basket basket = basketRepository.GetBasketByUserId(userId);

            // Clear the basket
            basketRepository.ClearBasket(basket.Id);
        }

        public bool ValidateBasketBeforeCheckOut(int basketId)
        {
            if (basketId <= NOBASKET)
            {
                throw new ArgumentException("Invalid basket ID");
            }

            // Get the basket items
            List<BasketItem> items = basketRepository.GetBasketItems(basketId);

            // Check if the basket is empty
            if (items.Count == NOBASKET)
            {
                return false;
            }

            // Check if all items have valid quantities
            foreach (BasketItem item in items)
            {
                if (item.Quantity <= NOQUANTITY)
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
            if (basketId <= NOBASKET)
            {
                throw new ArgumentException("Invalid basket ID");
            }
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException("Promo code cannot be empty");
            }

            // Convert to uppercase for case-insensitive comparison
            string normalizedCode = code.ToUpper().Trim();

            // Dictionary of valid promo codes
            Dictionary<string, float> validCodes = new Dictionary<string, float>
            {
                { "DISCOUNT10", 0.10f },  // 10% discount
                { "WELCOME20", 0.20f },
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
            if (string.IsNullOrWhiteSpace(code))
            {
                return NODISCOUNT;
            }

            // Convert to uppercase for case-insensitive comparison
            string normalizedCode = code.ToUpper().Trim();

            // Dictionary of valid promo codes
            Dictionary<string, float> validCodes = new Dictionary<string, float>
            {
                { "DISCOUNT10", 0.10f },  // 10% discount
                { "WELCOME20", 0.20f },
                { "FLASH30", 0.30f },     // 30% discount
            };

            // Check if the code exists in the valid codes
            if (validCodes.TryGetValue(normalizedCode, out float discountRate))
            {
                return subtotal * discountRate;
            }
            return NODISCOUNT;
        }

        // Add a new method to calculate basket totals
        public BasketTotals CalculateBasketTotals(int basketId, string promoCode)
        {
            if (basketId <= NOBASKET)
            {
                throw new ArgumentException("Invalid basket ID");
            }

            List<BasketItem> items = basketRepository.GetBasketItems(basketId);
            float subtotal = items.Sum(item => item.GetPrice());
            float discount = NODISCOUNT;

            if (!string.IsNullOrEmpty(promoCode))
            {
                discount = GetPromoCodeDiscount(promoCode, subtotal);
            }
                        float totalAmount = subtotal - discount;

            return new BasketTotals
            {
                Subtotal = subtotal,
                Discount = discount,
                TotalAmount = totalAmount
            };
        }
    }

    // Helper class to return the basket total values
    public class BasketTotals
    {
        public float Subtotal { get; set; }
        public float Discount { get; set; }
        public float TotalAmount { get; set; }
    }
}