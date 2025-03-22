using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Domain;

namespace DataAccessLayer.Repositories
{
    public abstract class ProductsRepository<T> where T : Product
    {
        public abstract List<T> GetProducts();
        public abstract T GetProductByID(int id);

        public abstract void AddProduct(T product);

        public abstract void UpdateProduct(T product);
        public abstract void DeleteProduct(T product);
    }
}
