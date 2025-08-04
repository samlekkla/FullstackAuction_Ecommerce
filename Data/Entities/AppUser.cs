using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AuctionCommerce.Data.Entities
{
    public class AppUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string DisplayName { get; set; } = string.Empty;

        public bool IsEmailVerified { get; set; } = false;
        public bool IsPhoneVerified { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<Auction> Auctions { get; set; } = new List<Auction>();
        public virtual ICollection<Bid> Bids { get; set; } = new List<Bid>();
    }
}
