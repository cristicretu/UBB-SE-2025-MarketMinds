using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Domain
{
    public class Bid
    {
        public User bidder { get; set; }
        public float price { get; set; }

        public DateTime timestamp { get; set; }

    }
}
