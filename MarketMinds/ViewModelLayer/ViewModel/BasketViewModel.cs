using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogicLayer.Services;
using DomainLayer.Domain;

namespace ViewModelLayer.ViewModel
{
    public class BasketViewModel
    {
        private User _currentUser;
        private readonly BasketService _basketService;
        private Basket _basket;

        // Properties exposed to the view
        public List<BasketItem> BasketItems { get; private set; }
        public float Subtotal { get; private set; }
        public float Discount { get; private set; }
        public float TotalAmount { get; private set; }
        public string PromoCode { get; set; }
        public string ErrorMessage { get; set; }

        // Constructor with dependencies
        public BasketViewModel(User currentUser, BasketService basketService)
        {
            _currentUser = currentUser;
            _basketService = basketService;
            BasketItems = new List<BasketItem>();
            PromoCode = string.Empty;
            ErrorMessage = string.Empty;
        }

        public void LoadBasket()
        {
            try
            {
                // Load basket from database through the service
                _basket = _basketService.GetBasketByUser(_currentUser);
                BasketItems = _basket.GetItems();

                // Calculate totals
                CalculateTotals();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load basket: {ex.Message}";
            }
        }

        public void RemoveProductFromBasket(int productId)
        {
            try
            {
                _basketService.RemoveProductFromBasket(_currentUser.Id, productId);
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

                    // Still update to the max quantity allowed
                    _basketService.UpdateProductQuantity(_currentUser.Id, productId, BasketService.MaxQuantityPerItem);
                }
                else
                {
                    ErrorMessage = string.Empty;
                    _basketService.UpdateProductQuantity(_currentUser.Id, productId, quantity);
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

                _basketService.ApplyPromoCode(_basket.Id, code);
                PromoCode = code;

                LoadBasket();
                ErrorMessage = string.Empty;
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
                _basketService.ClearBasket(_currentUser.Id);
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
            return _basketService.ValidateBasketBeforeCheckOut(_basket.Id);
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
            // Calculate subtotal
            Subtotal = BasketItems.Sum(item => item.GetPrice());

            // Apply any existing discount
            if (!string.IsNullOrEmpty(PromoCode))
            {
                if (PromoCode.ToUpper() == "DISCOUNT10")
                {
                    Discount = Subtotal * 0.1f;
                }
            }

            // Calculate total amount
            TotalAmount = Subtotal - Discount;
        }
    }
}