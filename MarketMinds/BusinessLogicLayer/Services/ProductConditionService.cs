using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
