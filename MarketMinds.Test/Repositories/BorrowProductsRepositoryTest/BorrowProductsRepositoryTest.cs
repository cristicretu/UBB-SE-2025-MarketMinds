using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using MarketMinds.Repositories.BorrowProductsRepository;
using DomainLayer.Domain;
using DataAccessLayer;
using Microsoft.Extensions.Configuration;
using MarketMinds.Test.Utils;

namespace MarketMinds.Tests.BorrowProductsRepositoryTest
{
    [TestFixture]
    public class BorrowProductRepositoryTest
    {
        private IBorrowProductsRepository borrowProductRepository;
        private DataBaseConnection connection;
        private IConfiguration config;
        private TestDatabaseHelper testDbHelper;

        User testSeller = new User(1, "test buyer", "test email");
        ProductCondition testProductCondition = new ProductCondition(1, "Test", "Test");
        ProductCategory testProductCategory = new ProductCategory(1, "test", "test");
        List<ProductTag> testProductTags = new List<ProductTag>();
        Product borrowProduct, auctionProduct;

        [SetUp]
        public void Setup()
        {
            config = TestDatabaseHelper.CreateTestConfiguration();
            testDbHelper = new TestDatabaseHelper(config);

            connection = new DataBaseConnection(config);
            borrowProductRepository = new BorrowProductsRepository(connection);

            testDbHelper.PrepareTestDatabase();

            DateTime timeLimit = DateTime.Now.AddDays(7);
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(5);
            bool isBorrowed = false;
            float dailyRate = 20.0f;

            borrowProduct = new BorrowProduct(1, "Test Product", "Test Description", testSeller,
                testProductCondition, testProductCategory, testProductTags, new List<Image>(),
                timeLimit, startDate, endDate, dailyRate, isBorrowed);

            auctionProduct = new AuctionProduct(1, "Test Product", "Test Description", testSeller, testProductCondition, testProductCategory, testProductTags, new List<Image>(), new DateTime(2025, 04, 04), new DateTime(2030, 04, 04), 100);
        }

        [Test]
        public void TestGetProducts_ReturnsListOfProducts()
        {
            var products = borrowProductRepository.GetProducts();

            Assert.That(products, Is.Not.Null);
            Assert.That(products.GetType(), Is.EqualTo(typeof(List<Product>)));
        }

        [Test]
        public void TestAddProduct_WrongProductType_ThrowsException()
        {
            Assert.Throws<InvalidCastException>(() => borrowProductRepository.AddProduct(auctionProduct));
        }

        [Test]
        public void TestAddProduct_ValidProduct_AddsProduct()
        {
            Assert.DoesNotThrow(() => borrowProductRepository.AddProduct(borrowProduct));
        }

        [Test]
        public void TestGetProductByID_ValidID_ReturnsProduct()
        {
            borrowProductRepository.AddProduct(borrowProduct);

            int productId = 1;
            var retrievedProduct = borrowProductRepository.GetProductByID(productId);

            Assert.That(retrievedProduct, Is.Not.Null);
            Assert.That(retrievedProduct.Id, Is.EqualTo(productId));
        }

        [Test]
        public void TestGetProductByID_InvalidID_ReturnsNull()
        {
            var retrievedProduct = borrowProductRepository.GetProductByID(999);
            Assert.That(retrievedProduct, Is.Null);
        }

        [Test]
        public void TestDeleteProduct_ValidProduct_DeletesProduct()
        {
            borrowProductRepository.AddProduct(borrowProduct);

            Assert.DoesNotThrow(() => borrowProductRepository.DeleteProduct(borrowProduct));
        }
    }
}
