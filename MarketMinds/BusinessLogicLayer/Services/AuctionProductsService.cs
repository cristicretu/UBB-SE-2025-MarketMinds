using DataAccessLayer.Repositories;
using DomainLayer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessLogicLayer.Services
{
    public class AuctionProductsService : ProductService
    {
        private AuctionProductsRepository _auctionRepository;
        
        public AuctionProductsService(AuctionProductsRepository repository): base(repository)
        {
            _auctionRepository = repository;
        }

        public void PlaceBid(AuctionProduct auction, User bidder, float bidAmount)
        {
            ValidateBid(auction, bidder, bidAmount);

            bidder.Balance -= bidAmount;

            RefundPreviousBidder(auction);

            var bid = new Bid (bidder, bidAmount, DateTime.Now);
            auction.AddBid(bid);
            auction.CurrentPrice = bidAmount;

            ExtendAuctionTime(auction);

            _auctionRepository.UpdateProduct(auction);

        }

        private void ValidateBid(AuctionProduct auction, User bidder, float bidAmount)
        {
            float minBid = auction.BidHistory.Count == 0 ? auction.StartingPrice : auction.CurrentPrice + 1;

            if (bidAmount < minBid)
                throw new Exception($"Bid must be at least ${minBid}");

            if (bidAmount > bidder.Balance)
                throw new Exception("Insufficient balance");

            if (DateTime.Now > auction.EndAuctionDate)
                throw new Exception("Auction already ended");
        }

        private void RefundPreviousBidder(AuctionProduct auction)
        {
            if (auction.BidHistory.Count > 0)
            {
                var previousBid = auction.BidHistory.Last();
                previousBid.Bidder.Balance += previousBid.Price;

            }
        }

        private void ExtendAuctionTime(AuctionProduct auction)
        {
            var timeRemaining = auction.EndAuctionDate - DateTime.Now;

            if (timeRemaining.TotalMinutes < 5)
            {
                auction.EndAuctionDate = auction.EndAuctionDate.AddMinutes(1);
            }
        }

        public void ConcludeAuction(AuctionProduct auction)
        {
            this._auctionRepository.DeleteProduct(auction);
        }

    }
}
