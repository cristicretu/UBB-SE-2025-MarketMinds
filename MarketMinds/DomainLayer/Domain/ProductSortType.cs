using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Domain
{
    public class ProductSortType
    {
        private static int nextId = 1;  
        public int id { get; set; }
        public string externalAttributeFieldTitle { get; set; } = string.Empty;
        public string internalAttributeFieldTitle { get; set; } = string.Empty;
        public bool isAscending { get; set; }

        public ProductSortType(string externalAttributeFieldTitle, string internalAttributeFieldTitle, bool isAscending)
        {
            this.id = nextId++;
            this.externalAttributeFieldTitle = externalAttributeFieldTitle;
            this.internalAttributeFieldTitle = internalAttributeFieldTitle;
            this.isAscending = isAscending;
        }

        public string getDisplayTitle()
        {
            return "Sort by " + this.externalAttributeFieldTitle + "(" + (this.isAscending ? "Ascending": "Descending") + ")";
        }

        public string getSqlOrderByStatement()
        {
            return "ORDER BY " + this.internalAttributeFieldTitle + (this.isAscending ? " ASC" : " DESC");
        }
    }
}
