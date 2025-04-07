using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Domain;
using MarketMinds.Repositories;

namespace MarketMinds.Services.ProductTagService
{
    public class ProductService : IProductService
    {
        private IProductsRepository productRepository;

        public ProductService(IProductsRepository repository)
        {
            productRepository = repository;
        }

        public List<Product> GetProducts()
        {
            return productRepository.GetProducts();
        }

        public Product GetProductById(int id)
        {
            return productRepository.GetProductByID(id);
        }

        public void AddProduct(Product product)
        {
            productRepository.AddProduct(product);
        }

        public void UpdateProduct(Product product)
        {
            productRepository.UpdateProduct(product);
        }

        public void DeleteProduct(Product product)
        {
            productRepository.DeleteProduct(product);
        }

        public static List<Product> GetSortedFilteredProducts(List<Product> products, List<ProductCondition> selectedConditions, List<ProductCategory> selectedCategories, List<ProductTag> selectedTags, ProductSortType sortCondition, string searchQuery)
        {
            List<Product> productResultSet = new List<Product>();
            foreach (Product product in products)
            {
                bool matchesConditions = selectedConditions == null || selectedConditions.Count == 0 || selectedConditions.Any(c => c.Id == product.Condition.Id);
                bool matchesCategories = selectedCategories == null || selectedCategories.Count == 0 || selectedCategories.Any(c => c.Id == product.Category.Id);
                bool matchesTags = selectedTags == null || selectedTags.Count == 0 || selectedTags.Any(t => product.Tags.Any(pt => pt.Id == t.Id));
                bool matchesSearchQuery = string.IsNullOrEmpty(searchQuery) || product.Title.ToLower().Contains(searchQuery.ToLower());

                if (matchesConditions && matchesCategories && matchesTags && matchesSearchQuery)
                {
                    productResultSet.Add(product);
                }
            }

            if (sortCondition != null)
            {
                if (sortCondition.IsAscending)
                {
                    productResultSet = productResultSet.OrderBy(
                        p =>
                        {
                            var prop = p?.GetType().GetProperty(sortCondition.InternalAttributeFieldTitle);
                            return prop?.GetValue(p);
                        }).ToList();
                }
                else
                {
                    productResultSet = productResultSet.OrderByDescending(
                        p =>
                        {
                            var prop = p?.GetType().GetProperty(sortCondition.InternalAttributeFieldTitle);
                            return prop?.GetValue(p);
                        }).ToList();
                }
            }
            return productResultSet;
        }
    }
}
