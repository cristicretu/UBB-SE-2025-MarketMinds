using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Domain
{
    public class AuctionProduct : Product
    {
        public DateTime StartAuctionDate { get; set; }
        public DateTime EndAuctionDate { get; set; }
        public float StartingPrice { get; set; }
        public float CurrentPrice { get; set; }

        public List<Bid> BidHistory { get; set; }

        public AuctionProduct(int Id, string Title, string Description, User Seller, ProductCondition ProductCondition, ProductCategory ProductCategory,
            List<ProductTag> ProductTags, List<Image> Images, DateTime StartAuctionDate, DateTime EndAuctionDate, float StartingPrice)
        {
            this.Id = Id;
            this.Description = Description;
            this.Title = Title;
            this.Seller = Seller;
            this.Condition = ProductCondition;
            this.Category = ProductCategory;
            this.Tags = ProductTags;
            this.Seller = Seller;
            this.Images = Images;
            this.StartAuctionDate = StartAuctionDate;
            this.EndAuctionDate = EndAuctionDate;
            this.StartingPrice = StartingPrice;
            this.CurrentPrice = StartingPrice;
            this.BidHistory = new List<Bid>();
        }

        public void AddBid(Bid bid)
        {
            this.CurrentPrice = bid.Price;
            this.BidHistory.Add(bid);
        }
    }
}
