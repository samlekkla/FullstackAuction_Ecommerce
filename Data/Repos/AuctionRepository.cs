using Microsoft.EntityFrameworkCore;
using AuctionCommerce.Data.Entities;
using AuctionCommerce.Data.Interfaces;
using AuctionCommerce.Data.Enums;

namespace AuctionCommerce.Data.Repos
{
    public class AuctionRepository : Repository<Auction>, IAuctionRepository
    {
        public AuctionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Auction>> GetActiveAuctionsAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Include(a => a.CreatedBy)
                .Include(a => a.Bids)
                .Where(a => a.Status == AuctionStatus.Active && 
                           a.StartDate <= now && 
                           a.EndDate > now)
                .OrderBy(a => a.EndDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Auction>> GetAuctionsByUserAsync(string userId)
        {
            return await _dbSet
                .Include(a => a.Bids)
                .Where(a => a.CreatedByUserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Auction?> GetAuctionWithBidsAsync(int auctionId)
        {
            return await _dbSet
                .Include(a => a.CreatedBy)
                .Include(a => a.Bids)
                    .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(a => a.Id == auctionId);
        }

        public async Task<IEnumerable<Auction>> GetEndingSoonAsync(int hours = 24)
        {
            var endTime = DateTime.UtcNow.AddHours(hours);
            var now = DateTime.UtcNow;
            
            return await _dbSet
                .Include(a => a.CreatedBy)
                .Include(a => a.Bids)
                .Where(a => a.Status == AuctionStatus.Active && 
                           a.EndDate > now && 
                           a.EndDate <= endTime)
                .OrderBy(a => a.EndDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Auction>> SearchAuctionsAsync(string searchTerm)
        {
            return await _dbSet
                .Include(a => a.CreatedBy)
                .Include(a => a.Bids)
                .Where(a => a.Status == AuctionStatus.Active &&
                           (a.Title.Contains(searchTerm) || 
                            a.Description.Contains(searchTerm)))
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }
    }
}
