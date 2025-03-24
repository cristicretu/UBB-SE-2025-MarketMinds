using DomainLayer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class BorrowProductsRepository : ProductsRepository
    {
        private DataBaseConnection connection;

        public BorrowProductsRepository(DataBaseConnection connection)
        {
            this.connection = connection;
        }

        public override void AddProduct(Product product)
        {
            throw new NotImplementedException();
        }

        public override void DeleteProduct(Product product)
        {
            throw new NotImplementedException();
        }

        public override Product GetProductByID(int id)
        {
            throw new NotImplementedException();
        }

        public override List<Product> GetProducts()
        {
            throw new NotImplementedException();
        }

        public override void UpdateProduct(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
