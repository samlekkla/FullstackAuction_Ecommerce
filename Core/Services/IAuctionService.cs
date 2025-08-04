using AuctionCommerce.Data.Entities;
using AuctionCommerce.Data.Interfaces;
using AuctionCommerce.Data.Enums;

namespace AuctionCommerce.Core.Services
{
    public interface IAuctionService
    {
        Task<IEnumerable<Auction>> GetActiveAuctionsAsync();
        Task<Auction?> GetAuctionByIdAsync(int id);
        Task<Auction> CreateAuctionAsync(Auction auction);
        Task<Auction?> UpdateAuctionAsync(int id, Auction auction);
        Task<bool> DeleteAuctionAsync(int id);
        Task<IEnumerable<Auction>> GetAuctionsByUserAsync(string userId);
        Task<IEnumerable<Auction>> GetEndingSoonAsync(int hours = 24);
        Task<IEnumerable<Auction>> SearchAuctionsAsync(string searchTerm);
        Task<bool> CanUserBidAsync(int auctionId, string userId);
        Task<bool> IsAuctionActiveAsync(int auctionId);
    }
}
