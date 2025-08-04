using AuctionCommerce.Data.Entities;

namespace AuctionCommerce.Data.Interfaces
{
    public interface IBidRepository : IRepository<Bid>
    {
        Task<IEnumerable<Bid>> GetBidsByAuctionAsync(int auctionId);
        Task<IEnumerable<Bid>> GetBidsByUserAsync(string userId);
        Task<Bid?> GetHighestBidForAuctionAsync(int auctionId);
        Task<IEnumerable<Bid>> GetWinningBidsForUserAsync(string userId);
        Task<decimal> GetCurrentPriceAsync(int auctionId);
        Task<bool> IsUserHighestBidderAsync(int auctionId, string userId);
        Task<int> GetBidCountForAuctionAsync(int auctionId);
    }
}
