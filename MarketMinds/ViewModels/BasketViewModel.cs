using System;
using System.Collections.Generic;
using System.Linq;
using DomainLayer.Domain;
using MarketMinds.Services.BasketService;

namespace ViewModelLayer.ViewModel
{
    public class BasketViewModel
    {
        private User currentUser;
        private readonly BasketService basketService;
        private Basket basket;

        public List<BasketItem> BasketItems { get; private set; }
        public float Subtotal { get; private set; }
        public float Discount { get; private set; }
        public float TotalAmount { get; private set; }
        public string PromoCode { get; set; }
        public string ErrorMessage { get; set; }

        public BasketViewModel(User currentUser, BasketService basketService)
        {
            this.currentUser = currentUser;
            this.basketService = basketService;
            BasketItems = new List<BasketItem>();
            PromoCode = string.Empty;
            ErrorMessage = string.Empty;
        }

        public void LoadBasket()
        {
            try
            {
                basket = basketService.GetBasketByUser(currentUser);
                BasketItems = basket.GetItems();

                // Calculate totals
                CalculateTotals();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load basket: {ex.Message}";
            }
        }

        public void AddToBasket(int productId)
        {
            try
            {
                basketService.AddProductToBasket(currentUser.Id, productId, 1);
                LoadBasket();
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to add product to basket: {ex.Message}";
            }
        }

        public void RemoveProductFromBasket(int productId)
        {
            try
            {
                basketService.RemoveProductFromBasket(currentUser.Id, productId);
                LoadBasket();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to remove product: {ex.Message}";
            }
        }

        public void UpdateProductQuantity(int productId, int quantity)
        {
            try
            {
                if (quantity > BasketService.MaxQuantityPerItem)
                {
                    ErrorMessage = $"Quantity cannot exceed {BasketService.MaxQuantityPerItem}";

                    basketService.UpdateProductQuantity(currentUser.Id, productId, BasketService.MaxQuantityPerItem);
                }
                else
                {
                    ErrorMessage = string.Empty;
                    basketService.UpdateProductQuantity(currentUser.Id, productId, quantity);
                }
                LoadBasket();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to update quantity: {ex.Message}";
            }
        }

        public void ApplyPromoCode(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    ErrorMessage = "Please enter a promo code.";
                    return;
                }

                basketService.ApplyPromoCode(basket.Id, code);
                PromoCode = code;

                CalculateTotals();
                ErrorMessage = $"Promo code '{code}' applied successfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to apply promo code: {ex.Message}";
                Discount = 0;
                TotalAmount = Subtotal;
            }
        }

        public void ClearBasket()
        {
            try
            {
                basketService.ClearBasket(currentUser.Id);
                LoadBasket();
                PromoCode = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to clear basket: {ex.Message}";
            }
        }

        public bool CanCheckout()
        {
            return basketService.ValidateBasketBeforeCheckOut(basket.Id);
        }

        public void Checkout()
        {
            if (!CanCheckout())
            {
                ErrorMessage = "Cannot checkout. Please check your basket items.";
                return;
            }

            ErrorMessage = string.Empty;
        }

        private void CalculateTotals()
        {
            Subtotal = BasketItems.Sum(item => item.GetPrice());

            if (!string.IsNullOrEmpty(PromoCode))
            {
                Discount = basketService.GetPromoCodeDiscount(PromoCode, Subtotal);
            }
            else
            {
                Discount = 0;
            }
            TotalAmount = Subtotal - Discount;
        }
    }
}