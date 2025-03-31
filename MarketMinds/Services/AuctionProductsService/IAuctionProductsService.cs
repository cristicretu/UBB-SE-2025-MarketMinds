using DomainLayer.Domain;

namespace MarketMinds.Services.AuctionProductsService
{
    /// <summary>
    /// Interface for the auction products service.
    /// </summary>
    public interface IAuctionProductsService
    {
        /// <summary>
        /// Places a bid on an auction product.
        /// </summary>
        /// <param name="auction">The auction product.</param>
        /// <param name="bidder">The user placing the bid.</param>
        /// <param name="bidAmount">The amount of the bid.</param>
        void PlaceBid(AuctionProduct auction, User bidder, float bidAmount);

        /// <summary>
        /// Concludes an auction.
        /// </summary>
        /// <param name="auction">The auction product to conclude.</param>
        void ConcludeAuction(AuctionProduct auction);
    }
}
