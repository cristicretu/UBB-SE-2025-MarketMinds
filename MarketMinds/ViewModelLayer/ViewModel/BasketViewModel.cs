using System;
using System.Collections.Generic;
using System.Linq;
using DomainLayer.Domain;

namespace ViewModelLayer.ViewModel
{
    public class BasketViewModel
    {
        private User _currentUser;

        // Properties exposed to the view
        public List<BasketItem> BasketItems { get; private set; }
        public float Subtotal { get; private set; }
        public float Discount { get; private set; }
        public float TotalAmount { get; private set; }
        public string PromoCode { get; set; }
        public string ErrorMessage { get; set; }

        // Constructor with dependencies
        public BasketViewModel(User currentUser)
        {
            _currentUser = currentUser;
            BasketItems = new List<BasketItem>();
            PromoCode = string.Empty;
            ErrorMessage = string.Empty;

            // Initialize with demo data (for testing purposes)
            InitializeDemoData();
        }

        // For demo - this should be replaced with actual service calls
        private void InitializeDemoData()
        {
            // Create a sample seller
            var seller = new User(2, "SampleSeller", "seller@example.com");

            // Create sample product condition
            var condition = new ProductCondition(1, "New", "Unopened, original packaging");

            // Create sample product category
            var category = new ProductCategory(1, "Electronics", "Electronic devices and accessories");

            // Create sample products
            var product1 = new BuyProduct(
                1,
                "Smartphone XYZ",
                "Latest model smartphone with high-resolution camera",
                seller,
                condition,
                category,
                new List<ProductTag>(),
                new List<Image>() { new Image("/Assets/placeholder.png") },
                499.99f);

            var product2 = new BuyProduct(
                2,
                "Wireless Headphones",
                "Noise-cancelling wireless headphones",
                seller,
                condition,
                category,
                new List<ProductTag>(),
                new List<Image>() { new Image("/Assets/placeholder.png") },
                129.99f);

            // Create basket items
            BasketItems.Add(new BasketItem(1, product1, 1) { Price = 499.99f });
            BasketItems.Add(new BasketItem(2, product2, 2) { Price = 129.99f });

            // Calculate totals
            CalculateTotals();
        }

        public void LoadBasket()
        {
            // In a real implementation, this would load from a service
            // For now, we're using the demo data that was initialized in the constructor
            CalculateTotals();
        }

        public void RemoveItem(int basketItemId)
        {
            var itemToRemove = BasketItems.FirstOrDefault(item => item.Id == basketItemId);
            if (itemToRemove != null)
            {
                BasketItems.Remove(itemToRemove);
                CalculateTotals();

                // Debug output to verify the item was removed
                System.Diagnostics.Debug.WriteLine($"Item {basketItemId} removed. Basket now has {BasketItems.Count} items.");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Item {basketItemId} not found in basket.");
            }
        }

        public void UpdateQuantity(int basketItemId, int newQuantity)
        {
            if (newQuantity <= 0)
            {
                RemoveItem(basketItemId);
                return;
            }

            var item = BasketItems.FirstOrDefault(i => i.Id == basketItemId);
            if (item != null)
            {
                item.Quantity = newQuantity;
                CalculateTotals();
            }
        }

        public void ApplyPromoCode(string code)
        {
            // Demo implementation - would usually call a service
            if (string.IsNullOrEmpty(code))
            {
                ErrorMessage = "Please enter a promo code.";
                return;
            }

            // Example promo code logic
            if (code.ToUpper() == "DISCOUNT10")
            {
                Discount = Subtotal * 0.1f; // 10% discount
                PromoCode = code;
                TotalAmount = Subtotal - Discount;
                ErrorMessage = string.Empty;
            }
            else
            {
                ErrorMessage = "Invalid promo code.";
                Discount = 0;
                TotalAmount = Subtotal;
            }
        }

        public void ClearBasket()
        {
            BasketItems.Clear();
            PromoCode = string.Empty;
            Discount = 0;
            CalculateTotals();
        }

        public bool CanCheckout()
        {
            return BasketItems.Count > 0 && BasketItems.All(item => item.HasValidPrice);
        }

        public void Checkout()
        {
            if (!CanCheckout())
            {
                ErrorMessage = "Cannot checkout. Please check your basket items.";
                return;
            }

            // This would typically start the checkout process
            // For now, just set a success message
            ErrorMessage = string.Empty;
        }

        private void CalculateTotals()
        {
            // Calculate subtotal
            Subtotal = BasketItems.Sum(item => item.GetPrice());

            // Apply any existing discount
            if (!string.IsNullOrEmpty(PromoCode) && PromoCode.ToUpper() == "DISCOUNT10")
            {
                Discount = Subtotal * 0.1f;
            }

            // Calculate total amount
            TotalAmount = Subtotal - Discount;
        }
    }
}