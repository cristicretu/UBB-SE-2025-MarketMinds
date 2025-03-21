using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Domain
{
    class AuctionProduct : Product
    {
        public DateTime startAuctionDate { get; set; }
        public DateTime endAuctionDate { get; set; }
        public float startingPrice { get; set; }
        public float currentPrice { get; set; }
        public User highestBidder { get; set; }
    }
}
