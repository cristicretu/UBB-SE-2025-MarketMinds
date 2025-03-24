using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Repositories;
using DomainLayer.Domain;

namespace BusinessLogicLayer.Services
{
    public class ProductService
    {
        private ProductsRepository productRepository;

        public ProductService(ProductsRepository repository)
        {
            this.productRepository = repository;
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
            //productRepository.UpdateProduct(product);
        }

        public void DeleteProduct(int id)
        {
            productRepository.DeleteProduct(id);
        }

        public List<Product> GetSortedFilteredProducts(List<ProductCondition> selectedConditions, List<ProductCategory> selectedCategories, List<ProductTag> selectedTags, ProductSortType sortCondition, string searchQuery)
        {
            List<Product> productResultSet = new List<Product>();
            foreach (Product product in this.GetProducts())
            {
                bool matchesConditions = selectedConditions == null || selectedConditions.Count == 0 || selectedConditions.Any(c => c.id == product.Condition.id);
                bool matchesCategories = selectedCategories == null || selectedCategories.Count == 0 || selectedCategories.Any(c => c.id == product.Category.id);
                bool matchesTags = selectedTags == null || selectedTags.Count == 0 || selectedTags.Any(t => product.Tags.Any(pt => pt.id == t.id));
                bool matchesSearchQuery = string.IsNullOrEmpty(searchQuery) || product.Title.ToLower().Contains(searchQuery.ToLower());

                if (matchesConditions && matchesCategories && matchesTags && matchesSearchQuery)
                {
                    productResultSet.Add(product);
                }
            }

            if (sortCondition != null)
            {
                if (sortCondition.isAscending)
                {
                    productResultSet = productResultSet.OrderBy(p => p.GetType().GetProperty(sortCondition.internalAttributeFieldTitle).GetValue(p, null)).ToList();
                }
                else
                {
                    productResultSet = productResultSet.OrderByDescending(p => p.GetType().GetProperty(sortCondition.internalAttributeFieldTitle).GetValue(p, null)).ToList();
                }
            }

            return productResultSet;
        }
    }
}
