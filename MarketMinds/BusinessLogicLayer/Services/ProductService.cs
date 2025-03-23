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
    public class ProductService<T> where T : Product
    {
        private ProductsRepository<T> productRepository;

        public ProductService(ProductsRepository<T> repository)
        {
            this.productRepository = repository;
        }

        public List<T> GetProducts()
        {
            return productRepository.GetProducts();
            
        }

        public T GetProductById(int id)
        {
            return productRepository.GetProductByID(id);
            
        }

        public void AddProduct(T product)
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

        public List<T> GetSortedFilteredProducts(List<ProductCondition> selectedConditions, List<ProductCategory> selectedCategories, List<ProductTag> selectedTags, ProductSortType sortCondition, string searchQuery)
        {
            List<T> productResultSet = new List<T>();
            foreach (Product product in this.GetProducts())
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
