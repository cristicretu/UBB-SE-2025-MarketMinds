using DomainLayer.Domain;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace MarketMinds.Test.Services.ProductTagService
{
    [TestFixture]
    internal class ProductServiceTests
    {
        private class TestProduct : Product
        {
        }

        [Test]
        public void GetProducts_ShouldReturnAllProducts()
        {
            var mockRepository = new ProductRepositoryMock();
            var productService = new MarketMinds.Services.ProductTagService.ProductService(mockRepository);

            var product1 = CreateSampleProduct(1, "Laptop");
            var product2 = CreateSampleProduct(2, "Smartphone");
            var product3 = CreateSampleProduct(3, "Tablet");

            mockRepository.AddProduct(product1);
            mockRepository.AddProduct(product2);
            mockRepository.AddProduct(product3);

            var result = productService.GetProducts();

            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result.Any(p => p.Id == 1 && p.Title == "Laptop"), Is.True);
            Assert.That(result.Any(p => p.Id == 2 && p.Title == "Smartphone"), Is.True);
            Assert.That(result.Any(p => p.Id == 3 && p.Title == "Tablet"), Is.True);
        }

        [Test]
        public void GetProductById_ShouldReturnCorrectProduct()
        {
            var mockRepository = new ProductRepositoryMock();
            var productService = new MarketMinds.Services.ProductTagService.ProductService(mockRepository);

            var product1 = CreateSampleProduct(1, "Laptop");
            var product2 = CreateSampleProduct(2, "Smartphone");

            mockRepository.AddProduct(product1);
            mockRepository.AddProduct(product2);

            var result = productService.GetProductById(2);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(2));
            Assert.That(result.Title, Is.EqualTo("Smartphone"));
        }

        [Test]
        public void AddProduct_ShouldAddProductToRepository()
        {
            var mockRepository = new ProductRepositoryMock();
            var productService = new MarketMinds.Services.ProductTagService.ProductService(mockRepository);
            var product = CreateSampleProduct(1, "Headphones");

            productService.AddProduct(product);

            Assert.That(mockRepository.Products.Count, Is.EqualTo(1));
            Assert.That(mockRepository.Products[0].Id, Is.EqualTo(1));
            Assert.That(mockRepository.Products[0].Title, Is.EqualTo("Headphones"));
        }

        [Test]
        public void DeleteProduct_ShouldRemoveProductFromRepository()
        {
            var mockRepository = new ProductRepositoryMock();
            var productService = new MarketMinds.Services.ProductTagService.ProductService(mockRepository);
            var product = CreateSampleProduct(1, "Keyboard");

            mockRepository.AddProduct(product);
            Assert.That(mockRepository.Products.Count, Is.EqualTo(1));

            productService.DeleteProduct(product);

            Assert.That(mockRepository.Products.Count, Is.EqualTo(0));
        }

        private Product CreateSampleProduct(int id, string title, ProductCategory category = null, ProductCondition condition = null)
        {
            return new TestProduct
            {
                Id = id,
                Title = title,
                Description = "Test product description",
                Category = category ?? new ProductCategory(1, "Default Category", "Default category description"),
                Condition = condition ?? new ProductCondition(1, "New", "Unused product in original packaging"),
                Tags = new List<ProductTag>(),
                Images = new List<Image>()
            };
        }
    }
}