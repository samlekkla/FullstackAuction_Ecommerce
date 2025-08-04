using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AuctionCommerce.Data.Enums;

namespace AuctionCommerce.Data.Entities
{
    public class Bid
    {
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public BidStatus Status { get; set; } = BidStatus.Active;

        // Foreign keys
        [Required]
        public int AuctionId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        // Navigation properties
        public virtual Auction Auction { get; set; } = null!;
        public virtual AppUser User { get; set; } = null!;
    }
}
