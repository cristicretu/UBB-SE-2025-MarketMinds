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
        private BuyProductsRepository buyProductsRepository;
        private AuctionProductsRepository auctionProductsRepository;

        public BasketService(
        BasketRepository repository,
        BuyProductsRepository buyProductsRepository,
        AuctionProductsRepository auctionProductsRepository)
        {
            this.repository = repository;
            this.buyProductsRepository = buyProductsRepository;
            this.auctionProductsRepository = auctionProductsRepository;
        }

        public Basket GetBasketByUser(User user)
        {
            if (user == null || user.Id <= 0)
            {
                throw new ArgumentException("Valid user must be provided");
            }

            // Get the user's basket or create one if it doesn't exist
            try
            {
                return repository.GetBasketByUser(user.Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to retrieve user's basket", ex);
            }
        }

        public void AddToBasket(int userId, int productId, int quantity)
        {
            if (userId <= 0) throw new ArgumentException("Invalid user ID");
            if (productId <= 0) throw new ArgumentException("Invalid product ID");
            if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero");

            var buyProduct = buyProductsRepository.GetBuyProductByID(productId);
            if (buyProduct == null)
            {
                throw new InvalidOperationException("Product not found or is not available for purchase");
            }

            // Check if there's enough stock available
            if (!IsStockAvailable(productId, quantity))
            {
                throw new InvalidOperationException("Not enough stock available");
            }

            // Get the user's basket
            Basket basket = repository.GetBasketByUser(userId);

            // Add the item to the basket
            repository.AddItemToBasket(basket.Id, productId, quantity);
        }

        private bool IsStockAvailable(int productId, int requestedQuantity)
        {
            // Check if the product exists
            try
            {
                var buyProduct = buyProductsRepository.GetBuyProductByID(productId);
                if (buyProduct == null)
                {
                    return false;
                }
                // Need to add a quantity field to BuyProduct and check it here

                // For now assuming unlimited stock:
                return true;

                // after adding quantity tracking:
                // return buyProduct.availableQuantity >= requestedQuantity;
            }
            catch
            {
                // Product not found
                return false;
            }
        }

        public void RemoveFromBasket(int userId, int basketItemId)
        {
            if (userId <= 0) throw new ArgumentException("Invalid user ID");
            if (basketItemId <= 0) throw new ArgumentException("Invalid basket item ID");

            // Get the user's basket
            Basket basket = repository.GetBasketByUser(userId);

            // Verify the item belongs to this user's basket before removing
            var basketItems = repository.GetBasketItems(basket.Id);
            bool itemBelongsToBasket = basketItems.Any(item => item.Id == basketItemId);

            if (!itemBelongsToBasket)
            {
                throw new UnauthorizedAccessException("The specified item does not belong to this user's basket");
            }

            // Remove the item from the basket
            repository.RemoveItemFromBasket(basketItemId);
        }

        public void UpdateQuantity(int userId, int basketItemId, int quantity)
        {
            if (userId <= 0) throw new ArgumentException("Invalid user ID");
            if (basketItemId <= 0) throw new ArgumentException("Invalid basket item ID");
            if (quantity < 0) throw new ArgumentException("Quantity cannot be negative");

            // Get the user's basket
            Basket basket = repository.GetBasketByUser(userId);

            // Verify the item belongs to this user's basket
            var basketItems = repository.GetBasketItems(basket.Id);
            var itemToUpdate = basketItems.FirstOrDefault(item => item.Id == basketItemId);

            if (itemToUpdate == null)
            {
                throw new UnauthorizedAccessException("The specified item does not belong to this user's basket");
            }

            // Check if new quantity is available in stock
            if (quantity > 0 && !IsStockAvailable(itemToUpdate.Product.Id, quantity))
            {
                throw new InvalidOperationException("Not enough stock available for the requested quantity");
            }

            // Update the item quantity
            if (quantity == 0)
            {
                repository.RemoveItemFromBasket(basketItemId);
            }
            else
            {
                repository.UpdateItemQuantity(basketItemId, quantity);
            }
        }

        public void ClearBasket(int userId)
        {
            if (userId <= 0) throw new ArgumentException("Invalid user ID");

            // Get the user's basket
            Basket basket = repository.GetBasketByUser(userId);

            // Clear the basket
            repository.ClearBasket(basket.Id);
        }

        public bool ValidateBasketBeforeCheckOut(int basketId)
        {
            if (basketId <= 0) throw new ArgumentException("Invalid basket ID");

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

                // Check if product is of valid type
                if (!item.HasValidPrice)
                {
                    return false;
                }

                var product = buyProductsRepository.GetBuyProductByID(item.Product.Id);
                if (product == null)
                {
                    return false;
                }

                // Check if price is still valid (hasn't changed)
                if (product is BuyProduct buyProduct && Math.Abs(buyProduct.Price - item.Price) > 0.001f)
                {
                    // Price has changed since item was added to basket
                    return false;
                }

                // Check if stock is available again
                if (!IsStockAvailable(item.Product.Id, item.Quantity))
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

            // This method needs to be implemented
            throw new NotImplementedException("ApplyPromoCode needs to be implemented");
        }
    }
}