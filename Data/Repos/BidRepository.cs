using Microsoft.EntityFrameworkCore;
using AuctionCommerce.Data.Entities;
using AuctionCommerce.Data.Interfaces;
using AuctionCommerce.Data.Enums;

namespace AuctionCommerce.Data.Repos
{
    public class BidRepository : Repository<Bid>, IBidRepository
    {
        public BidRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Bid>> GetBidsByAuctionAsync(int auctionId)
        {
            return await _dbSet
                .Include(b => b.User)
                .Where(b => b.AuctionId == auctionId)
                .OrderByDescending(b => b.Amount)
                .ThenByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<Bid?> GetHighestBidForAuctionAsync(int auctionId)
        {
            return await _dbSet
                .Include(b => b.User)
                .Where(b => b.AuctionId == auctionId && b.Status == BidStatus.Active)
                .OrderByDescending(b => b.Amount)
                .ThenBy(b => b.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Bid>> GetBidsByUserAsync(string userId)
        {
            return await _dbSet
                .Include(b => b.Auction)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Bid>> GetWinningBidsForUserAsync(string userId)
        {
            // Get bids where user is the highest bidder for completed auctions
            return await _dbSet
                .Include(b => b.Auction)
                .Include(b => b.User)
                .Where(b => b.UserId == userId && 
                           b.Status == BidStatus.Active &&
                           b.Auction.Status == AuctionStatus.Completed)
                .GroupBy(b => b.AuctionId)
                .Select(g => g.OrderByDescending(b => b.Amount).First())
                .ToListAsync();
        }

        public async Task<decimal> GetCurrentPriceAsync(int auctionId)
        {
            var highestBid = await GetHighestBidForAuctionAsync(auctionId);
            if (highestBid != null)
            {
                return highestBid.Amount;
            }

            // Return starting price if no bids
            var auction = await _context.Set<Auction>()
                .FirstOrDefaultAsync(a => a.Id == auctionId);
            
            return auction?.StartingPrice ?? 0;
        }

        public async Task<bool> IsUserHighestBidderAsync(int auctionId, string userId)
        {
            var highestBid = await GetHighestBidForAuctionAsync(auctionId);
            return highestBid?.UserId == userId;
        }

        public async Task<int> GetBidCountForAuctionAsync(int auctionId)
        {
            return await _dbSet
                .CountAsync(b => b.AuctionId == auctionId && b.Status == BidStatus.Active);
        }
    }
}
