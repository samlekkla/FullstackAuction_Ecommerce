using AuctionCommerce.Data.Entities;
using AuctionCommerce.Data.Interfaces;
using AuctionCommerce.Data.Enums;
using AuctionCommerce.Core.Services;

namespace AuctionCommerce.Core.Services
{
    public class BidService : IBidService
    {
        private readonly IBidRepository _bidRepository;
        private readonly IAuctionService _auctionService;

        public BidService(IBidRepository bidRepository, IAuctionService auctionService)
        {
            _bidRepository = bidRepository;
            _auctionService = auctionService;
        }

        public async Task<IEnumerable<Bid>> GetBidsByAuctionAsync(int auctionId)
        {
            return await _bidRepository.GetBidsByAuctionAsync(auctionId);
        }

        public async Task<Bid?> GetHighestBidForAuctionAsync(int auctionId)
        {
            return await _bidRepository.GetHighestBidForAuctionAsync(auctionId);
        }

        public async Task<Bid> PlaceBidAsync(Bid bid)
        {
            // Validate that the bid can be placed
            var canBid = await CanPlaceBidAsync(bid.AuctionId, bid.UserId, bid.Amount);
            if (!canBid)
                throw new InvalidOperationException("Cannot place this bid");

            bid.CreatedAt = DateTime.UtcNow;
            bid.Status = BidStatus.Active;

            await _bidRepository.AddAsync(bid);
            return bid;
        }

        public async Task<IEnumerable<Bid>> GetBidsByUserAsync(string userId)
        {
            return await _bidRepository.GetBidsByUserAsync(userId);
        }

        public async Task<decimal> GetCurrentPriceAsync(int auctionId)
        {
            return await _bidRepository.GetCurrentPriceAsync(auctionId);
        }

        public async Task<bool> IsUserHighestBidderAsync(int auctionId, string userId)
        {
            return await _bidRepository.IsUserHighestBidderAsync(auctionId, userId);
        }

        public async Task<IEnumerable<Bid>> GetWinningBidsForUserAsync(string userId)
        {
            return await _bidRepository.GetWinningBidsForUserAsync(userId);
        }

        public async Task<bool> CanPlaceBidAsync(int auctionId, string userId, decimal amount)
        {
            // Check if user can bid on this auction
            var canUserBid = await _auctionService.CanUserBidAsync(auctionId, userId);
            if (!canUserBid)
                return false;

            // Check if auction is still active
            var isActive = await _auctionService.IsAuctionActiveAsync(auctionId);
            if (!isActive)
                return false;

            // Check if bid amount is higher than current price
            var currentPrice = await GetCurrentPriceAsync(auctionId);
            if (amount <= currentPrice)
                return false;

            // Check if user is not already the highest bidder
            var isHighestBidder = await IsUserHighestBidderAsync(auctionId, userId);
            if (isHighestBidder)
                return false;

            return true;
        }
    }
}
