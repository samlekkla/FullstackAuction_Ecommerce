using AuctionCommerce.Data.Entities;

namespace AuctionCommerce.Core.Services
{
    public interface IBidService
    {
        Task<IEnumerable<Bid>> GetBidsByAuctionAsync(int auctionId);
        Task<Bid?> GetHighestBidForAuctionAsync(int auctionId);
        Task<Bid> PlaceBidAsync(Bid bid);
        Task<IEnumerable<Bid>> GetBidsByUserAsync(string userId);
        Task<decimal> GetCurrentPriceAsync(int auctionId);
        Task<bool> IsUserHighestBidderAsync(int auctionId, string userId);
        Task<bool> CanPlaceBidAsync(int auctionId, string userId, decimal amount);
        Task<IEnumerable<Bid>> GetWinningBidsForUserAsync(string userId);
    }
}
