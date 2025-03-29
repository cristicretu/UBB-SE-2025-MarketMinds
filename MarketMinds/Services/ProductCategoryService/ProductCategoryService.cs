using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Domain;
using MarketMinds.Repositories.ProductCategoryRepository;

namespace MarketMinds.Services.ProductCategoryService
{
    public class ProductCategoryService : IProductCategoryService
    {
        private ProductCategoryRepository repository;

        public ProductCategoryService(ProductCategoryRepository repository)
        {
            this.repository = repository;
        }

        public List<ProductCategory> GetAllProductCategories()
        {
            return repository.GetAllProductCategories();
        }

        public ProductCategory CreateProductCategory(string displayTitle, string description)
        {
            return repository.CreateProductCategory(displayTitle, description);
        }

        public void DeleteProductCategory(string displayTitle)
        {
            repository.DeleteProductCategory(displayTitle);
        }
    }
}
