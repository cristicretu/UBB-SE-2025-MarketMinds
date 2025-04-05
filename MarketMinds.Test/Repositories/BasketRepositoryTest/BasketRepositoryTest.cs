using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using DomainLayer.Domain;
using MarketMinds.Repositories.AuctionProductsRepository;
using MarketMinds.Repositories.BasketRepository;
using MarketMinds.Repositories.BuyProductsRepository;
using MarketMinds.Test.Utils;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace MarketMinds.Test.Repositories.BasketRepositoryTest
{
    [TestFixture]
    public class BasketRepositoryTest
    {
        private IBasketRepository basketRepository;
        private IBuyProductsRepository buyProductRepository;
        private DataBaseConnection connection;
        private IConfiguration config;
        private TestDatabaseHelper testDbHelper;

        User testUser = new User(1, "test buyer", "test email");
        Basket testBasket;
        Product testProduct;

        [SetUp]
        public void Setup()
        {
            config = TestDatabaseHelper.CreateTestConfiguration();
            testDbHelper = new TestDatabaseHelper(config);

            connection = new DataBaseConnection(config);
            basketRepository = new BasketRepository(connection);
            buyProductRepository = new BuyProductsRepository(connection);

            testDbHelper.PrepareTestDatabase();
             
            testProduct = new BuyProduct(1, "Test Product", "Test Description", new User(2, "test seller", "test seller email"), new ProductCondition(1, "New", "New"), new ProductCategory(1, "Electronics", "Electic"), new List<ProductTag>(), new List<Image>(), 100);
            testBasket = new Basket(1);
        }

        [Test]
        public void TestGetBasketByUserId_InvalidUserId_ThrowsException()
        {
            int invalidUserId = -1;

            Assert.Catch<Microsoft.Data.SqlClient.SqlException>(() => basketRepository.GetBasketByUserId(invalidUserId));
        }

        [Test]
        public void TestGetBasketByUserId_ValidUserId_RetursnBasketWithItems()
        {
            int userId = testUser.Id;

            testBasket = basketRepository.GetBasketByUserId(userId);

            Assert.That(testBasket, Is.Not.Null);
        }

        [Test]
        public void TestAddItemToBasket_InvalidProduct_ThrowsException()
        {
            int basketId = testBasket.Id;
            int productId = -1;
            int quantity = 2;

            Assert.Throws<ArgumentException>(() => basketRepository.AddItemToBasket(basketId, productId, quantity));
        }

        [Test]
        public void TestAddItemToBasket_ValidProduct_AddsItem()
        {
            testBasket = basketRepository.GetBasketByUserId(testUser.Id);

            buyProductRepository.AddProduct(testProduct);

            int basketId = testBasket.Id;
            int productId = testProduct.Id;
            int quantity = 2;

            basketRepository.AddItemToBasket(basketId, productId, quantity);

            List<BasketItem> items = basketRepository.GetBasketItems(basketId);
            Assert.That(items, Is.Not.Null);
            Assert.That(items.Any(i => i.Product.Id == productId && i.Quantity == quantity), Is.True);
        }

        [Test]
        public void TestGetBasketItems_InvalidBasketId_ReturnsNullItemsList()
        {
            int invalidBasketId = -1;

            List<BasketItem> items = basketRepository.GetBasketItems(invalidBasketId);

            Assert.That(items, Is.Empty);
        }

        [Test]
        public void TestGetBasketItems_ValidBasketId_ReturnsItems()
        {
            int basketId = testBasket.Id;
            List<BasketItem> items = basketRepository.GetBasketItems(basketId);
            Assert.That(items, Is.Not.Null);
            Assert.That(items.Count, Is.Not.Null);
        }

        [Test]
        public void TestUpdateItemQuantityByProductId_InvalidProductId_ThrowsException()
        {
            int basketId = testBasket.Id;
            int invalidProductId = -1;
            int newQuantity = 2;
            Assert.Throws<ArgumentException>(() => basketRepository.UpdateItemQuantityByProductId(basketId, invalidProductId, newQuantity));
        }

        [Test]
        public void TestUpdateItemQuantityByProductId_ValidProductId_UpdatesQuantity()
        {
            testBasket = basketRepository.GetBasketByUserId(testUser.Id);
            int basketId = testBasket.Id;

            var product1 = new BuyProduct(
                1,
                "Test Product 1",
                "Test Description",
                new User(2, "test seller", "test seller email"),
                new ProductCondition(1, "New", "New"),
                new ProductCategory(1, "Electronics", "Electric"),
                new List<ProductTag>(),
                new List<Image>(),
                100);

            var product2 = new BuyProduct(
                2,
                "Test Product 2",
                "Test Description",
                new User(2, "test seller", "test seller email"),
                new ProductCondition(1, "New", "New"),
                new ProductCategory(1, "Electronics", "Electric"),
                new List<ProductTag>(),
                new List<Image>(),
                200);

            buyProductRepository.AddProduct(product1);
            buyProductRepository.AddProduct(product2);

            basketRepository.AddItemToBasket(basketId, product1.Id, 1);
            basketRepository.AddItemToBasket(basketId, product2.Id, 1);

            int productIdToUpdate = product1.Id;
            int newQuantity = 3;
            basketRepository.UpdateItemQuantityByProductId(basketId, productIdToUpdate, newQuantity);

            List<BasketItem> items = basketRepository.GetBasketItems(basketId);
            Assert.That(items, Is.Not.Null);
            Assert.That(items.Count, Is.EqualTo(2), "Basket should contain 2 different products");

            BasketItem updatedItem = items.FirstOrDefault(i => i.Product.Id == productIdToUpdate);
            Assert.That(updatedItem, Is.Not.Null);
            Assert.That(updatedItem.Quantity, Is.EqualTo(newQuantity));
        }

        [Test]
        public void TestUpdateItemQuantityByProductId_InvalidQuantity_ThrowsException()
        {
            int basketId = testBasket.Id;
            int productId = testProduct.Id;
            int invalidQuantity = -1;
            Assert.Throws<ArgumentException>(() => basketRepository.UpdateItemQuantityByProductId(basketId, productId, invalidQuantity));
        }

        [Test]
        public void TestClearBasket_InvalidBasketId_ThrowsException()
        {
            int invalidBasketId = -1;
            Assert.Throws<ArgumentException>(() => basketRepository.ClearBasket(invalidBasketId));
        }

        [Test]
        public void TestClearBasket_ValidBasketId_ClearsBasket()
        {
            int basketId = 1;

            basketRepository.ClearBasket(basketId);
            List<BasketItem> items = basketRepository.GetBasketItems(basketId);

            Assert.That(items, Is.Not.Null);
            Assert.That(items.Count, Is.EqualTo(0));
        }


        [Test]
        public void TestRemoveItemByProductId_InvalidProductId_DoesNotThrow()
        {
            int basketId = testBasket.Id;
            int invalidProductId = -1;

            Assert.DoesNotThrow(() => basketRepository.RemoveItemByProductId(basketId, invalidProductId));
        }

        [Test]
        public void TestRemoveItemByProductId_ValidProductId_RemovesItem()
        {
            int basketId = testBasket.Id;
            int productId = testProduct.Id;

            basketRepository.RemoveItemByProductId(basketId, productId);

            List<BasketItem> items = basketRepository.GetBasketItems(basketId);
            Assert.That(items, Is.Not.Null);
            Assert.That(items.Any(i => i.Product.Id == productId), Is.False);
        }
    }
}
