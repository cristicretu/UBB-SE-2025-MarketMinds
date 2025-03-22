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
        //private ProductRepository productRepository;

        public ProductService()
        {
            //productRepository = new ProductRepository();
        }

        public List<Product> GetAllProducts()
        {
            //return productRepository.GetAllProducts();
            return null;
        }

        public Product GetProductById(int id)
        {
            //return productRepository.GetProductById(id);
            return null;
        }

        public void AddProduct(Product product)
        {
            //productRepository.AddProduct(product);
        }

        public void UpdateProduct(Product product)
        {
            //productRepository.UpdateProduct(product);
        }

        public void DeleteProduct(int id)
        {
            //productRepository.DeleteProduct(id);
        }

        public List<Product> GetSortedFilteredProducts(List<ProductCondition> selectedConditions, List<ProductCategory> selectedCategories, List<ProductTag> selectedTags, ProductSortType sortCondition, string searchQuery)
        {
            List<Product> productResultSet = new List<Product>();
            foreach (Product product in this.GetAllProducts())
            {
                bool matchesConditions = selectedConditions == null || selectedConditions.Count == 0 || selectedConditions.Any(c => c.id == product.condition.id);
                bool matchesCategories = selectedCategories == null || selectedCategories.Count == 0 || selectedCategories.Any(c => c.id == product.category.id);
                bool matchesTags = selectedTags == null || selectedTags.Count == 0 || selectedTags.Any(t => product.tags.Any(pt => pt.id == t.id));
                bool matchesSearchQuery = string.IsNullOrEmpty(searchQuery) || product.title.ToLower().Contains(searchQuery.ToLower());

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
