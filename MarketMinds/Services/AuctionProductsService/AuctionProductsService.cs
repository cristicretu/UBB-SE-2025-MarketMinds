using System.Threading.Tasks;
using System.Text;
using System;
using System.Linq;
using System.Collections.Generic;
using DomainLayer.Domain;
using MarketMinds.Repositories.AuctionProductsRepository;
using MarketMinds.Services.ProductTagService;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MarketMinds.Services.AuctionProductsService
{
    public class AuctionProductsService : ProductService, IAuctionProductsService
    {
        private IAuctionProductsRepository auctionRepository;
        private const int BIDCOUNT = 0;
        private const int MINUTES_TO_EXTEND = 5;

        public AuctionProductsService(IAuctionProductsRepository repository) : base(repository)
        {
            auctionRepository = repository;
        }

        public void CreateListing(Product product)
        {
            AddProduct(product);
        }

        public void PlaceBid(AuctionProduct auction, User bidder, float bidAmount)
        {
            ValidateBid(auction, bidder, bidAmount);

            bidder.Balance -= bidAmount;

            RefundPreviousBidder(auction);

            var bid = new Bid(bidder, bidAmount, DateTime.Now);
            auction.AddBid(bid);
            auction.CurrentPrice = bidAmount;

            ExtendAuctionTime(auction);

            auctionRepository.UpdateProduct(auction);
        }
        private void ValidateBid(AuctionProduct auction, User bidder, float bidAmount)
        {
            float minBid = auction.BidHistory.Count == BIDCOUNT ? auction.StartingPrice : auction.CurrentPrice + 1;

            if (bidAmount < minBid)
            {
                throw new Exception($"Bid must be at least ${minBid}");
            }

            if (bidAmount > bidder.Balance)
            {
                throw new Exception("Insufficient balance");
            }

            if (DateTime.Now > auction.EndAuctionDate)
            {
                throw new Exception("Auction already ended");
            }
        }

        private void RefundPreviousBidder(AuctionProduct auction)
        {
            if (auction.BidHistory.Count > BIDCOUNT)
            {
                var previousBid = auction.BidHistory.Last();
                previousBid.Bidder.Balance += previousBid.Price;
            }
        }
        private void ExtendAuctionTime(AuctionProduct auction)
        {
            var timeRemaining = auction.EndAuctionDate - DateTime.Now;

            if (timeRemaining.TotalMinutes < MINUTES_TO_EXTEND)
            {
                auction.EndAuctionDate = auction.EndAuctionDate.AddMinutes(MINUTES_TO_EXTEND);
            }
        }

        public void ConcludeAuction(AuctionProduct auction)
        {
            auctionRepository.DeleteProduct(auction);
        }
    }
}
