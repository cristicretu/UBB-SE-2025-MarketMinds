﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Domain;
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

        public List<ProductTag> GetAllProductTags()
        {
            return repository.GetAllProductTags();
        }

        public ProductTag CreateProductTag(string displayTitle)
        {
            return repository.CreateProductTag(displayTitle);
        }

        public void DeleteProductTag(string displayTitle)
        {
            repository.DeleteProductTag(displayTitle);
        }
    }
}
