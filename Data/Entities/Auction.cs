using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AuctionCommerce.Data.Enums;

namespace AuctionCommerce.Data.Entities
{
    public class Auction
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal StartingPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ReservePrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CurrentHighestBid { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [Required]
        public AuctionStatus Status { get; set; } = AuctionStatus.Draft;

        // Foreign key
        [Required]
        public string CreatedByUserId { get; set; } = string.Empty;

        // Navigation properties
        public virtual AppUser CreatedBy { get; set; } = null!;
        public virtual ICollection<Bid> Bids { get; set; } = new List<Bid>();

        // Computed properties
        public bool IsActive => Status == AuctionStatus.Active && DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;
        public TimeSpan TimeRemaining => EndDate > DateTime.UtcNow ? EndDate - DateTime.UtcNow : TimeSpan.Zero;
    }
}
