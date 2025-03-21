using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Domain;
using DataAccessLayer.Repositories;

namespace BusinessLogicLayer.Services
{
    class ProductCategoryService
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
