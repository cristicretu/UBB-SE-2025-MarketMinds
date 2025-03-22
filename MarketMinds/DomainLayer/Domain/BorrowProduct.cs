using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Domain
{
    class BorrowProduct : Product
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public TimeSpan timeLimit
        {
            get
            {
                return endDate - startDate;
            }
        }
        public BorrowProduct(int id, string description, string title, ProductCondition condition, ProductCategory category, List<ProductTag> tags, DateTime startDate, DateTime endDate) : base(id, description, title, condition, category, tags)
        {
            this.startDate = startDate;
            this.endDate = endDate;
        }
    }
}
