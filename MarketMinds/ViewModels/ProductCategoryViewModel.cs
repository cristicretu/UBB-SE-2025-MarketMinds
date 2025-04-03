using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Domain;
using MarketMinds.Services.ProductCategoryService;

namespace ViewModelLayer.ViewModel
{
    public class ProductCategoryViewModel
    {
        private ProductCategoryService productCategoryService;

        public ProductCategoryViewModel(ProductCategoryService productCategoryService)
        {
            this.productCategoryService = productCategoryService;
        }
        public List<ProductCategory> GetAllProductCategories()
        {
            return productCategoryService.GetAllProductCategories();
        }

        public ProductCategory CreateProductCategory(string displayTitle, string description)
        {
            return productCategoryService.CreateProductCategory(displayTitle, description);
        }

        public void DeleteProductCategory(string displayTitle)
        {
            productCategoryService.DeleteProductCategory(displayTitle);
        }
    }
}
