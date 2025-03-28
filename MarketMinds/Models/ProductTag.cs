using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Domain
{
    public class ProductTag
    {
        public int id { get; set; }
        public string displayTitle { get; set; } = string.Empty;

        public ProductTag(int id, string displayTitle)
        {
            this.id = id;
            this.displayTitle = displayTitle;
        }
    }
}
