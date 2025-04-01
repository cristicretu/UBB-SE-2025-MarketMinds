using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Domain
{
    public class BorrowProduct : Product
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime TimeLimit { get; set; }
        public float DailyRate { get; set; }
        public bool IsBorrowed { get; set; }
        public BorrowProduct(int Id, string Title, string Description, User Seller, ProductCondition ProductCondition, ProductCategory ProductCategory,
            List<ProductTag> ProductTags, List<Image> Images, DateTime TimeLimit, DateTime StartDate, DateTime EndDate, float DailyRate, bool IsBorrowed)
        {
            this.Id = Id;
            this.Title = Title;
            this.Description = Description;
            this.Seller = Seller;
            this.Condition = ProductCondition;
            this.Category = ProductCategory;
            this.Tags = ProductTags;
            this.Images = Images;
            this.StartDate = StartDate;
            this.EndDate = EndDate;
            this.TimeLimit = TimeLimit;
            this.DailyRate = DailyRate;
            this.IsBorrowed = IsBorrowed;
        }
    }
}
