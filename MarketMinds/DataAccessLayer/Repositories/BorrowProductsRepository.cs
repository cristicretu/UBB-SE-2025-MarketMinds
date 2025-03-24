using DomainLayer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class BuyProductsRepository : ProductsRepository
    {
        private DataBaseConnection connection;

        public AuctionProductsRepository(DataBaseConnection connection)
        {
            this.connection = connection;
        }
        public BuyProduct GetBorrowProductByID(int productId)
        {
            // Implementation to get auction product by ID
            return null;
        }
    }
}
