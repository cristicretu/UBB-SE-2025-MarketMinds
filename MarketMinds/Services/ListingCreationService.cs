using System;
using DomainLayer.Domain;
using MarketMinds.Services.BuyProductsService;
using MarketMinds.Services.BorrowProductsService;
using MarketMinds.Services.AuctionProductsService;

namespace MarketMinds.Services
{
    public class ListingCreationService
    {
        private readonly BuyProductsService buyProductsService;
        private readonly BorrowProductsService borrowProductsService;
        private readonly AuctionProductsService auctionProductsService;

        public ListingCreationService(
            BuyProductsService buyProductsService,
            BorrowProductsService borrowProductsService,
            AuctionProductsService auctionProductsService)
        {
            this.buyProductsService = buyProductsService;
            this.borrowProductsService = borrowProductsService;
            this.auctionProductsService = auctionProductsService;
        }

        public void CreateListing(Product product, string listingType)
        {
            switch (listingType.ToLower())
            {
                case "buy":
                    buyProductsService.CreateListing(product);
                    break;
                case "borrow":
                    borrowProductsService.CreateListing(product);
                    break;
                case "auction":
                    auctionProductsService.CreateListing(product);
                    break;
                default:
                    throw new ArgumentException($"Invalid listing type: {listingType}");
            }
        }
    }
} 