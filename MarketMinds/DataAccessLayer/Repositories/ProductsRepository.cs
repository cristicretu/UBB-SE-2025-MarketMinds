using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Domain;

namespace DataAccessLayer.Repositories
{
    public abstract class ProductsRepository
    {
        public abstract List<Product> GetProducts();
        public abstract Product GetProductByID(int id);

        public abstract void AddProduct(Product product);

        public abstract void UpdateProduct(Product product);
        public abstract void DeleteProduct(Product product);
    }
}
