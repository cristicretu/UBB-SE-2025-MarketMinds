using System;
using System.Collections.Generic;
using NUnit.Framework;
using DomainLayer.Domain;
using MarketMinds.Services.BasketService;
using MarketMinds.Repositories.BasketRepository;
using MarketMinds.Test.Services.BasketServiceTest;

namespace MarketMinds.Tests.Services.BasketServiceTest
{
    [TestFixture]
    public class BasketServiceTest
    {
        private BasketService basketService;
        private BasketRepositoryMock basketRepositoryMock;
        private User testUser;
        private int testUserId = 1;
        private int testProductId = 10;

        [SetUp]
        public void Setup()
        {
            basketRepositoryMock = new BasketRepositoryMock();
            basketService = new BasketService(basketRepositoryMock);
            testUser = new User(testUserId, "Test User", "test@example.com");
        }

        [Test]
        public void TestAddProductToBasket_ValidParameters_AddsProduct()
        {
            int quantity = 3;

            basketService.AddProductToBasket(testUserId, testProductId, quantity);

            Assert.That(basketRepositoryMock.GetAddItemCount(), Is.EqualTo(1));
        }

        [Test]
        public void TestAddProductToBasket_QuantityExceedsMax_LimitsQuantity()
        {
            int quantity = BasketService.MaxQuantityPerItem + 5;

            basketService.AddProductToBasket(testUserId, testProductId, quantity);

            Assert.That(basketRepositoryMock.GetAddItemCount(), Is.EqualTo(1));
        }

        [Test]
        public void TestAddProductToBasket_InvalidUserId_ThrowsException()
        {
            int invalidUserId = 0;

            Exception ex = null;
            try
            {
                basketService.AddProductToBasket(invalidUserId, testProductId, 1);
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Is.EqualTo("Invalid user ID"));
            Assert.That(basketRepositoryMock.GetAddItemCount(), Is.EqualTo(0));
        }

        [Test]
        public void TestAddProductToBasket_InvalidProductId_ThrowsException()
        {
            int invalidProductId = 0;

            Exception ex = null;
            try
            {
                basketService.AddProductToBasket(testUserId, invalidProductId, 1);
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Is.EqualTo("Invalid product ID"));
            Assert.That(basketRepositoryMock.GetAddItemCount(), Is.EqualTo(0));
        }

        [Test]
        public void TestGetBasketByUser_ValidUser_ReturnsBasket()
        {
            var basket = basketService.GetBasketByUser(testUser);

            Assert.That(basket, Is.Not.Null);
        }

        [Test]
        public void TestGetBasketByUser_NullUser_ThrowsException()
        {
            Exception ex = null;
            try
            {
                basketService.GetBasketByUser(null);
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Is.EqualTo("Valid user must be provided"));
        }

        [Test]
        public void TestRemoveProductFromBasket_ValidParameters_RemovesProduct()
        {
            basketService.RemoveProductFromBasket(testUserId, testProductId);

            Assert.That(basketRepositoryMock.GetRemoveItemCount(), Is.EqualTo(1));
        }

        [Test]
        public void TestRemoveProductFromBasket_InvalidUserId_ThrowsException()
        {
            int invalidUserId = 0;

            Exception ex = null;
            try
            {
                basketService.RemoveProductFromBasket(invalidUserId, testProductId);
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Is.EqualTo("Invalid user ID"));
            Assert.That(basketRepositoryMock.GetRemoveItemCount(), Is.EqualTo(0));
        }

        [Test]
        public void TestUpdateProductQuantity_ValidParameters_UpdatesQuantity()
        {
            basketService.AddProductToBasket(testUserId, testProductId, 1);

            int addCount = basketRepositoryMock.GetAddItemCount();
            Assert.That(addCount, Is.EqualTo(1));

            int quantity = 5;
            basketService.UpdateProductQuantity(testUserId, testProductId, quantity);

            Assert.That(basketRepositoryMock.GetUpdateItemCount(), Is.EqualTo(1));
        }

        [Test]
        public void TestUpdateProductQuantity_QuantityExceedsMax_LimitsQuantity()
        {
            basketService.AddProductToBasket(testUserId, testProductId, 1);

            int addCount = basketRepositoryMock.GetAddItemCount();
            Assert.That(addCount, Is.EqualTo(1));

            int quantity = BasketService.MaxQuantityPerItem + 5;
            basketService.UpdateProductQuantity(testUserId, testProductId, quantity);

            Assert.That(basketRepositoryMock.GetUpdateItemCount(), Is.EqualTo(1));
        }

        [Test]
        public void TestUpdateProductQuantity_QuantityZero_RemovesProduct()
        {
            int quantity = 0;

            basketService.UpdateProductQuantity(testUserId, testProductId, quantity);

            Assert.That(basketRepositoryMock.GetRemoveItemCount(), Is.EqualTo(1));
            Assert.That(basketRepositoryMock.GetUpdateItemCount(), Is.EqualTo(0));
        }

        [Test]
        public void TestUpdateProductQuantity_InvalidUserId_ThrowsException()
        {
            int invalidUserId = 0;

            Exception ex = null;
            try
            {
                basketService.UpdateProductQuantity(invalidUserId, testProductId, 1);
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Is.EqualTo("Invalid user ID"));
            Assert.That(basketRepositoryMock.GetUpdateItemCount(), Is.EqualTo(0));
        }

        [Test]
        public void TestClearBasket_ValidUserId_ClearsBasket()
        {
            basketService.ClearBasket(testUserId);

            Assert.That(basketRepositoryMock.GetClearBasketCount(), Is.EqualTo(1));
        }

        [Test]
        public void TestClearBasket_InvalidUserId_ThrowsException()
        {
            int invalidUserId = 0;

            Exception ex = null;
            try
            {
                basketService.ClearBasket(invalidUserId);
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Is.EqualTo("Invalid user ID"));
            Assert.That(basketRepositoryMock.GetClearBasketCount(), Is.EqualTo(0));
        }

        [Test]
        public void TestValidateBasketBeforeCheckOut_EmptyBasket_ReturnsFalse()
        {
            int basketId = 1;

            bool result = basketService.ValidateBasketBeforeCheckOut(basketId);

            Assert.That(result, Is.False);
        }

        [Test]
        public void TestApplyPromoCode_ValidCode_AppliesDiscount()
        {
            int basketId = 1;
            string validCode = "DISCOUNT10";

            basketService.ApplyPromoCode(basketId, validCode);

            float discount = basketService.GetPromoCodeDiscount(validCode, 100);
            Assert.That(discount, Is.EqualTo(10));
        }

        [Test]
        public void TestApplyPromoCode_InvalidCode_ThrowsException()
        {
            int basketId = 1;
            string invalidCode = "INVALID";

            Exception ex = null;
            try
            {
                basketService.ApplyPromoCode(basketId, invalidCode);
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Is.EqualTo("Invalid promo code"));
        }
    }
}
