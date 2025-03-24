using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Domain
{
    public class BuyProduct : Product
    {
        public float Price { get; set; }
        public BuyProduct(int Id, string Title, string Description, User Seller, ProductCondition ProductCondition, ProductCategory ProductCategory,
            List<ProductTag> ProductTags, List<Image> Images, float Price)
        {
            this.Id = Id;
            this.Title = Title;
            this.Description = Description;
            this.Seller = Seller;
            this.Condition = ProductCondition;
            this.Category = ProductCategory;
            this.Tags = ProductTags;
            this.Seller = Seller;
            this.Images = Images;
            this.Price = Price;
        }
    }
}
