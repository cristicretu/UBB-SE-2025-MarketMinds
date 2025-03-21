using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
