using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using MarketMinds.Repositories.AuctionProductsRepository;
using DataAccessLayer;
using Microsoft.Extensions.Configuration;

namespace MarketMinds.Tests
{
    [TestFixture]
    public class AuctionProductRepositoryTest
    {
        private IAuctionProductsRepository? auctionProductRepository;
        private DataBaseConnection? connection;
        private IConfiguration? config;

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
        public void TestGetProducts()
        {
            // Arrange
            var expectedCount = 5; // Adjust this based on your test data
            // Act
            var products = auctionProductRepository.GetProducts();
            // Assert
            Assert.That(products, Is.Not.Null);
            Assert.That(products.Count, Is.EqualTo(expectedCount));
        }

        [Test]
        public void TestYourMom()
        {
            // Arrange
            var expected = 1;
            // Act
            var actual = 1;
            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}