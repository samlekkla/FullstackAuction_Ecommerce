using System.ComponentModel.DataAnnotations;
using AuctionCommerce.Data.Enums;

namespace AuctionCommerce.DTOs
{
    public class BidCreateDto
    {
        [Required]
        public int AuctionId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Bid amount must be greater than 0")]
        public decimal Amount { get; set; }
    }

    public class BidResponseDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public BidStatus Status { get; set; }
        public int AuctionId { get; set; }
        public string AuctionTitle { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserDisplayName { get; set; } = string.Empty;
        public bool IsHighestBid { get; set; }
    }
}
