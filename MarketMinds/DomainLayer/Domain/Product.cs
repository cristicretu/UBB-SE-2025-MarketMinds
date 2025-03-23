using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Domain
{
    public class Product
    {

        public virtual float getPrice()
        {
            throw new NotImplementedException("getPrice() not implemented for the base Product class.");
        }
<<<<<<< HEAD
        public Product(int id, string description, string title, ProductCondition condition, ProductCategory category, List<ProductTag> tags)
        {
            this.id = id;
            this.description = description;
            this.title = title;
            this.condition = condition;
            this.category = category;
            this.tags = tags;
        }
=======
        public int Id { get; set; }
        public string Description { get; set; }
        public ProductCondition Condition { get; set; }
        public ProductCategory Category { get; set; }
        public List<ProductTag> Tags { get; set; }

        public User Seller { get; set; }
        public Product(int Id, User Seller , string Description, ProductCondition Condition, ProductCategory Category, List<ProductTag> Tags)
        {
            this.Id = Id;
            this.Seller = Seller;
            this.Description = Description;
            this.Condition = Condition;
            this.Category = Category;
            this.Tags = Tags;
            this.Seller = Seller;
        }

>>>>>>> main
    }
}
