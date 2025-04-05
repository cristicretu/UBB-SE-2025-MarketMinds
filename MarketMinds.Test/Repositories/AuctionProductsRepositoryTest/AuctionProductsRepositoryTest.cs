using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using MarketMinds.Repositories.AuctionProductsRepository;
using DomainLayer.Domain;
using DataAccessLayer;
using Microsoft.Extensions.Configuration;
using MarketMinds.Test.Utils;

namespace MarketMinds.Tests.AuctionProductsRepositoryTest
{
    [TestFixture]
    public class AuctionProductRepositoryTest
    {
        private IAuctionProductsRepository auctionProductRepository;
        private DataBaseConnection connection;
        private IConfiguration config;
        private TestDatabaseHelper testDbHelper;

        User testSeller = new User(1, "test buyer", "test email");
        ProductCondition testProductCondition = new ProductCondition(1, "Test", "Test");
        ProductCategory testProductCategory = new ProductCategory(1, "test", "test");
        List<ProductTag> testProductTags = new List<ProductTag>();
        Product auctionProduct, buyProduct;

        [SetUp]
        public void Setup()
        {
            config = TestDatabaseHelper.CreateTestConfiguration();
            testDbHelper = new TestDatabaseHelper(config);

            connection = new DataBaseConnection(config);
            auctionProductRepository = new AuctionProductsRepository(connection);
                
            testDbHelper.PrepareTestDatabase();

            auctionProduct = new AuctionProduct(1, "Test Product", "Test Description", testSeller, testProductCondition, testProductCategory, testProductTags, new List<Image>(), new DateTime(2025, 04, 04), new DateTime(2030, 04, 04), 100);
            buyProduct = new BuyProduct(1, "Test Product", "Test Description", testSeller, testProductCondition, testProductCategory, testProductTags, new List<Image>(), 100);
        }

        [Test]
        public void TestGetProducts_ReturnsListOfProducts()
        {
            var products = auctionProductRepository.GetProducts();

            Assert.That(products, Is.Not.Null);
            Assert.That(products.GetType(), Is.EqualTo(typeof(List<Product>)));
        }

        [Test]
        public void TestAddProduct_WrongProductType_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => auctionProductRepository.AddProduct(buyProduct));
        }

        [Test]
        public void TestAddProduct_ValidProduct_AddsProduct()
        {
            Assert.DoesNotThrow(() => auctionProductRepository.AddProduct(auctionProduct));
        }

        [Test]
        public void TestGetProductByID_ValidID_ReturnsProduct()
        {
            auctionProductRepository.AddProduct(auctionProduct);

            int productId = 1;
            var retrievedProduct = auctionProductRepository.GetProductByID(productId);

            Assert.That(retrievedProduct, Is.Not.Null);
            Assert.That(retrievedProduct.Id, Is.EqualTo(productId));
        }

        [Test]
        public void TestGetProductByID_InvalidID_ReturnsNull()
        {
            var retrievedProduct = auctionProductRepository.GetProductByID(999);
            Assert.That(retrievedProduct, Is.Null);
        }

        [Test]
        public void TestUpdateProduct_NullProduct_ThrowsException()
        {
            Product product = null;
            Assert.Throws<ArgumentException>(() => auctionProductRepository.UpdateProduct(product));
        }

        [Test]
        public void TestUpdateProductPrice_ValidProductPrice_UpdatesProductPrice()
        {
            auctionProductRepository.AddProduct(auctionProduct);

            AuctionProduct updatedProduct = new AuctionProduct(auctionProduct.Id, auctionProduct.Title, auctionProduct.Description, auctionProduct.Seller, auctionProduct.Condition, auctionProduct.Category, auctionProduct.Tags, auctionProduct.Images, new DateTime(2025, 04, 04), new DateTime(2030, 04, 04), 1000f);

            Assert.DoesNotThrow(() => auctionProductRepository.UpdateProduct(auctionProduct));

            updatedProduct = auctionProductRepository.GetProductByID(auctionProduct.Id);
            Assert.That(updatedProduct, Is.Not.Null, "Product was not found after update");
            Assert.That(updatedProduct.CurrentPrice, Is.EqualTo(100));
        }

        [Test]
        public void TestDeleteProduct_WrongProductType_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => auctionProductRepository.DeleteProduct(buyProduct));
        }

        [Test]
        public void TestDeleteProduct_ValidProduct_DeletesProduct()
        {

            Assert.DoesNotThrow(() => auctionProductRepository.DeleteProduct(auctionProduct));

            var retrievedProduct = auctionProductRepository.GetProductByID(auctionProduct.Id);
            Assert.That(retrievedProduct, Is.Null);
        }
    }
}