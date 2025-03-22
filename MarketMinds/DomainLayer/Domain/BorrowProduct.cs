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
        public BorrowProduct(DateTime startDate, DateTime endDate)
        {
            this.startDate = startDate;
            this.endDate = endDate;
        }
    }
}
