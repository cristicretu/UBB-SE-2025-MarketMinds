using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Repositories;

namespace BusinessLogicLayer.Services
{
    public class ProductTagService
    {
        private ProductTagRepository repository;

        public ProductTagService(ProductTagRepository repository)
        {
            this.repository = repository;
        }
    }
}
