using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Domain
{
    public class Product
    {
        public int id { get; set; }
        public string description { get; set; }
        public string title { get; set; }
        public ProductCondition condition { get; set; }
        public ProductCategory category { get; set; }
        public List<ProductTag> tags { get; set; }
        public virtual float getPrice()
        {
            throw new NotImplementedException("getPrice() not implemented for the base Product class.");
        }
        public Product(int id, string description, string title, ProductCondition condition, ProductCategory category, List<ProductTag> tags)
        {
            this.id = id;
            this.description = description;
            this.title = title;
            this.condition = condition;
            this.category = category;
            this.tags = tags;
        }
    }
}
