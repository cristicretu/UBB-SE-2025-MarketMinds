using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Domain
{
    public class ProductCondition
    {
        public int id { get; set; }
        public string displayTitle { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;

        public ProductCondition(int id, string displayTitle, string description)
        {
            this.id = id;
            this.displayTitle = displayTitle;
            this.description = description;
        }
    }
}
