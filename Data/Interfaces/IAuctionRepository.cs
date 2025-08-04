using AuctionCommerce.Data.Entities;

namespace AuctionCommerce.Data.Interfaces
{
    public interface IAuctionRepository : IRepository<Auction>
    {
        Task<IEnumerable<Auction>> GetActiveAuctionsAsync();
        Task<IEnumerable<Auction>> GetAuctionsByUserAsync(string userId);
        Task<Auction?> GetAuctionWithBidsAsync(int auctionId);
        Task<IEnumerable<Auction>> GetEndingSoonAsync(int hours = 24);
        Task<IEnumerable<Auction>> SearchAuctionsAsync(string searchTerm);
    }
}
