using System;
using System.Collections.Generic;
using BusinessLogicLayer.Services;
using DomainLayer.Domain;

namespace ViewModelLayer.ViewModel
{
    public class BasketViewModel
    {
        private readonly BasketService basketService;
        private User currentUser;
        private Basket basket;

        public List<BasketItem> BasketItems { get; private set; }
        public float Subtotal { get; private set; }
        public float Discount { get; private set; }
        public float TotalAmount { get; private set; }
        public string PromoCode { get; set; }
        public string ErrorMessage { get; set; }

        public BasketViewModel(BasketService basketService, User currentUser)
        {
            this.basketService = basketService;
            this.currentUser = currentUser;
            this.BasketItems = new List<BasketItem>();
            this.PromoCode = string.Empty;
            this.ErrorMessage = string.Empty;

            // Load the basket data
            LoadBasket();
        }

        public void LoadBasket()
        {
            // Get the basket from the service
            basket = basketService.GetBasketByUser(currentUser);

            // Update the basket items
            BasketItems = basket.GetItems();

            // Calculate totals
            CalculateTotals();
        }

        public void RemoveItem(int basketItemId)
        {
            basketService.RemoveFromBasket(currentUser.Id, basketItemId);

            // Reload the basket to see the changes
            LoadBasket();
        }

        public void UpdateQuantity(int basketItemId, int newQuantity)
        {
            basketService.UpdateQuantity(currentUser.Id, basketItemId, newQuantity);

            // Reload the basket to see the changes
            LoadBasket();
        }

        public void ApplyPromoCode(string code)
        {
            basketService.ApplyPromoCode(basket.Id, code);

            // Reload the basket to see the changes
            LoadBasket();

            // Update the promo code
            PromoCode = code;
        }

        public void ClearBasket()
        {
            basketService.ClearBasket(currentUser.Id);

            // Reload the basket to see the changes
            LoadBasket();
        }

        public void ContinueShopping()
        {
            // Redirect to the products page
        }

        public bool CanCheckout()
        {
            return basketService.ValidateBasketBeforeCheckOut(basket.Id);
        }

        public void Checkout()
        {
            // Simple validation before proceeding
            if (!CanCheckout())
            {
                ErrorMessage = "Cannot checkout. Please check your basket items.";
                return;
            }

            // Redirect to the checkout page
        }

        private void CalculateTotals()
        {
            // Calculate subtotal
            Subtotal = basket.GetTotal();

            // Calculate discount
            Discount = 0; // For now, no discount is applied

            // Calculate total amount
            TotalAmount = Subtotal - Discount;
        }
    }
}