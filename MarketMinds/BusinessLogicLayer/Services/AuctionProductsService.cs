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
    public class AuctionProductsService : ProductService<AuctionProduct>
    {
        private AuctionProductsRepository _auctionRepository;
        public AuctionProductsService(AuctionProductsRepository repository)
            : base(repository)
        {
            _auctionRepository = repository;
        }

        public void PlaceBid(AuctionProduct auction, User bidder, float bidAmount)
        {
            ValidateBid(auction, bidder, bidAmount);

            bidder.Balance -= bidAmount;

            RefundPreviousBidder(auction);

            var bid = new Bid { Bidder = bidder, Price = bidAmount, Timestamp = DateTime.Now };
            auction.AddBid(bid);
            auction.CurrentPrice = bidAmount;

            ExtendAuctionTime(auction);

            _auctionRepository.UpdateProduct(auction);

            Notify(bidder, $"Your bid of ${bidAmount} is placed successfully.");
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

                Notify(previousBid.Bidder, $"You've been outbid. ${previousBid.Price} has been refunded to your account.");
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

        private void Notify(User user, string message)
        {
            return;
        }

        public void ConcludeAuction(AuctionProduct auction, User seller)
        {
            if (auction.BidHistory.Count == 0)
            {
                Notify(seller, $"The auction for {auction.Id} has ended. Unfortunately, no bids were placed.");
                return;
            }

            var winningBid = auction.BidHistory.Last();
            Notify(winningBid.Bidder, $"Congratulations! You have won the auction for {auction.Id} with a bid of ${winningBid.Price}.");
            Notify(seller, $"The auction for {auction.Id} has ended. The highest bid was ${winningBid.Price} placed by {winningBid.Bidder.Username}.");

            seller.Balance += winningBid.Price;

            CreateOrder(winningBid.Bidder, auction);
        }

        private void CreateOrder(User buyer, AuctionProduct product)
        {
            return;
        }
       
    }
}
