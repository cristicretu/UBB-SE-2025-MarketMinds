using System;
using System.Collections.Generic;
using DomainLayer.Domain;

namespace MarketMinds.Services.BasketService
{
    /// <summary>
    /// Interface for BasketService to manage basket operations.
    /// </summary>
    public interface IBasketService
    {
        /// <summary>
        /// Adds a product to the user's basket.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="productId">The ID of the product to add.</param>
        /// <param name="quantity">The quantity of the product to add.</param>
        void AddToBasket(int userId, int productId, int quantity);

        /// <summary>
        /// Retrieves the user's basket.
        /// </summary>
        /// <param name="user">The user object.</param>
        /// <returns>The user's basket.</returns>
        Basket GetBasketByUser(User user);

        /// <summary>
        /// Removes a product from the user's basket.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="productId">The ID of the product to remove.</param>
        void RemoveProductFromBasket(int userId, int productId);

        /// <summary>
        /// Updates the quantity of a product in the user's basket.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="productId">The ID of the product to update.</param>
        /// <param name="quantity">The new quantity of the product.</param>
        void UpdateProductQuantity(int userId, int productId, int quantity);

        /// <summary>
        /// Clears all items from the user's basket.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        void ClearBasket(int userId);

        /// <summary>
        /// Validates the basket before checkout.
        /// </summary>
        /// <param name="basketId">The ID of the basket.</param>
        /// <returns>True if the basket is valid for checkout, otherwise false.</returns>
        bool ValidateBasketBeforeCheckOut(int basketId);

        /// <summary>
        /// Applies a promo code to the basket.
        /// </summary>
        /// <param name="basketId">The ID of the basket.</param>
        /// <param name="code">The promo code to apply.</param>
        void ApplyPromoCode(int basketId, string code);

        /// <summary>
        /// Gets the discount for a promo code.
        /// </summary>
        /// <param name="code">The promo code.</param>
        /// <param name="subtotal">The subtotal amount.</param>
        /// <returns>The discount amount.</returns>
        float GetPromoCodeDiscount(string code, float subtotal);
    }
}
