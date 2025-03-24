using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.Services;
using DomainLayer.Domain;

namespace ViewModelLayer.ViewModel
{
    public class ProductConditionViewModel
    {
        private ProductConditionService productConditionService;

        public ProductConditionViewModel(ProductConditionService productConditionService)
        {
            this.productConditionService = productConditionService;
        }
        
        public List<ProductCondition> GetAllProductConditions()
        {
            return productConditionService.GetAllProductConditions();
        }

        public ProductCondition CreateProductCondition(string displayTitle, string description)
        {
            return productConditionService.CreateProductCondition(displayTitle, description);
        }

        public void DeleteProductCondition(string displayTitle)
        {
            productConditionService.DeleteProductCondition(displayTitle);
        }
    }
}
