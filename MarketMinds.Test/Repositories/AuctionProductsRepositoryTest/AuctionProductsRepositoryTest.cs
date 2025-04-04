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

namespace MarketMinds.Tests.AuctionProductsRepositoryTest
{
    [TestFixture]
    public class AuctionProductRepositoryTest
    {
        private IAuctionProductsRepository auctionProductRepository;
        private DataBaseConnection connection;
        private IConfiguration config;

        [SetUp]
        public void Setup()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            config = builder.Build();

            connection = new DataBaseConnection(config);
            auctionProductRepository = new AuctionProductsRepository(connection);
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
            var product = new BuyProduct(1, "Test Product", "Test Description", new User(2, "test seller", "test email"), new ProductCondition(1, "Test", "Test"), new ProductCategory(1, "test", "test"), new List<ProductTag>(), new List<Image>(), 100);

            Assert.Throws<ArgumentException>(() => auctionProductRepository.AddProduct(product));
        }

        [Test]
        public void TestAddProduct_ValidProduct_AddsProduct()
        {
            var product = new AuctionProduct(1, "Test Product", "Test Description", new User(2, "test seller", "test email"), new ProductCondition(1, "Test", "Test"), new ProductCategory(1, "test", "test"), new List<ProductTag>(), new List<Image>(), new DateTime(2025, 04, 04), new DateTime(2030, 04, 04), 100);

            Assert.DoesNotThrow(() => auctionProductRepository.AddProduct(product));
        }

        [Test]
        public void TestDeleteProduct_WrongProductType_ThrowsException() {
            var product = new BuyProduct(1, "Test Product", "Test Description", new User(2, "test seller", "test email"), new ProductCondition(1, "Test", "Test"), new ProductCategory(1, "test", "test"), new List<ProductTag>(), new List<Image>(), 100);
            
            Assert.Throws<ArgumentException>(() => auctionProductRepository.DeleteProduct(product));
        }

        [Test]
        public void TestDeleteProduct_ValidProduct_DeletesProduct()
        {
            var product = new AuctionProduct(1, "Test Product", "Test Description", new User(2, "test seller", "test email"), new ProductCondition(1, "Test", "Test"), new ProductCategory(1, "test", "test"), new List<ProductTag>(), new List<Image>(), new DateTime(2025, 04, 04), new DateTime(2030, 04, 04), 100);
            
            auctionProductRepository.AddProduct(product);
            
            Assert.DoesNotThrow(() => auctionProductRepository.DeleteProduct(product));
        }

        [Test]
        public void TestUpdateProduct_NullProduct_ThrowsException()
        {
            Product product = null;
            Assert.Throws<ArgumentException>(() => auctionProductRepository.UpdateProduct(product));
        }

        [Test]
        public void TestUpdateProduct_ValidProduct_UpdatesProduct()
        {
            var product = new AuctionProduct(1, "Test Product", "Test Description", new User(2, "test seller", "test email"), new ProductCondition(1, "Test", "Test"), new ProductCategory(1, "test", "test"), new List<ProductTag>(), new List<Image>(), new DateTime(2025, 04, 04), new DateTime(2030, 04, 04), 100);

            auctionProductRepository.AddProduct(product);

            product.Title = "Updated Title";
            Assert.DoesNotThrow(() => auctionProductRepository.UpdateProduct(product));
        }

        [Test]
        public void TestGetProductByID_ValidID_ReturnsProduct()
        {
            int productId = 2;
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
    }
}