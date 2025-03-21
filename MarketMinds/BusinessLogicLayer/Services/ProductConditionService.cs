using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Domain;
using DataAccessLayer.Repositories;

namespace BusinessLogicLayer.Services
{
    public class ProductConditionService
    {
        private ProductConditionRepository repository;

        public ProductConditionService(ProductConditionRepository repository)
        {
            this.repository = repository;
        }

        public List<ProductCondition> GetAllProductConditions()
        {
            return repository.GetAllProductConditions();
        }

        public ProductCondition CreateProductCondition(string displayTitle, string description)
        {
            return repository.CreateProductCondition(displayTitle, description);
        }

        public void DeleteProductCondition(string displayTitle)
        {
            repository.DeleteProductCondition(displayTitle);
        }
    }
}
