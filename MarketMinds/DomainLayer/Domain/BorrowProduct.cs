using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Domain
{
    class BorrowProduct : Product
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan TimeLimit { get; set; }
        public BorrowProduct(int Id, string Title, string Description, User Seller, ProductCondition ProductCondition, ProductCategory ProductCategory,
            List<ProductTag> ProductTags, List<Image> Images, DateTime StartDate, DateTime EndDate)
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
            this.StartDate = StartDate;
            this.EndDate = EndDate;
            this.TimeLimit = EndDate - StartDate;
        }
    }
}
