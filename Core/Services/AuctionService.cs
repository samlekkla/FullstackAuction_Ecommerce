using AuctionCommerce.Data.Entities;
using AuctionCommerce.Data.Interfaces;
using AuctionCommerce.Data.Enums;
using AuctionCommerce.Core.Services;

namespace AuctionCommerce.Core.Services
{
    public class AuctionService : IAuctionService
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly IBidRepository _bidRepository;

        public AuctionService(IAuctionRepository auctionRepository, IBidRepository bidRepository)
        {
            _auctionRepository = auctionRepository;
            _bidRepository = bidRepository;
        }

        public async Task<IEnumerable<Auction>> GetActiveAuctionsAsync()
        {
            return await _auctionRepository.GetActiveAuctionsAsync();
        }

        public async Task<Auction?> GetAuctionByIdAsync(int id)
        {
            return await _auctionRepository.GetAuctionWithBidsAsync(id);
        }

        public async Task<Auction> CreateAuctionAsync(Auction auction)
        {
            auction.CreatedAt = DateTime.UtcNow;
            auction.Status = AuctionStatus.Active;
            
            if (auction.StartDate < DateTime.UtcNow)
                auction.StartDate = DateTime.UtcNow;

            await _auctionRepository.AddAsync(auction);
            return auction;
        }

        public async Task<Auction?> UpdateAuctionAsync(int id, Auction auction)
        {
            var existingAuction = await _auctionRepository.GetByIdAsync(id);
            if (existingAuction == null)
                return null;

            // Only allow updates if auction hasn't started or has no bids
            var hasBids = await _bidRepository.GetBidCountForAuctionAsync(id) > 0;
            if (existingAuction.StartDate <= DateTime.UtcNow && hasBids)
                throw new InvalidOperationException("Cannot update auction that has started and has bids");

            existingAuction.Title = auction.Title;
            existingAuction.Description = auction.Description;
            existingAuction.StartingPrice = auction.StartingPrice;
            existingAuction.StartDate = auction.StartDate;
            existingAuction.EndDate = auction.EndDate;
            existingAuction.UpdatedAt = DateTime.UtcNow;

            await _auctionRepository.UpdateAsync(existingAuction);
            return existingAuction;
        }

        public async Task<bool> DeleteAuctionAsync(int id)
        {
            var auction = await _auctionRepository.GetByIdAsync(id);
            if (auction == null)
                return false;

            // Only allow deletion if auction hasn't started or has no bids
            var hasBids = await _bidRepository.GetBidCountForAuctionAsync(id) > 0;
            if (auction.StartDate <= DateTime.UtcNow && hasBids)
                throw new InvalidOperationException("Cannot delete auction that has started and has bids");

            await _auctionRepository.DeleteAsync(auction);
            return true;
        }

        public async Task<IEnumerable<Auction>> GetAuctionsByUserAsync(string userId)
        {
            return await _auctionRepository.GetAuctionsByUserAsync(userId);
        }

        public async Task<IEnumerable<Auction>> GetEndingSoonAsync(int hours = 24)
        {
            return await _auctionRepository.GetEndingSoonAsync(hours);
        }

        public async Task<IEnumerable<Auction>> SearchAuctionsAsync(string searchTerm)
        {
            return await _auctionRepository.SearchAuctionsAsync(searchTerm);
        }

        public async Task<bool> CanUserBidAsync(int auctionId, string userId)
        {
            var auction = await _auctionRepository.GetByIdAsync(auctionId);
            if (auction == null)
                return false;

            // User cannot bid on their own auction
            if (auction.CreatedByUserId == userId)
                return false;

            // Auction must be active and within time range
            return await IsAuctionActiveAsync(auctionId);
        }

        public async Task<bool> IsAuctionActiveAsync(int auctionId)
        {
            var auction = await _auctionRepository.GetByIdAsync(auctionId);
            if (auction == null)
                return false;

            var now = DateTime.UtcNow;
            return auction.Status == AuctionStatus.Active &&
                   auction.StartDate <= now &&
                   auction.EndDate > now;
        }
    }
}
