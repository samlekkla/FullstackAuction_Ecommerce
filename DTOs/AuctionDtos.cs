using System.ComponentModel.DataAnnotations;
using AuctionCommerce.Data.Enums;

namespace AuctionCommerce.DTOs
{
    public class AuctionCreateDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Starting price must be greater than 0")]
        public decimal StartingPrice { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }

    public class AuctionUpdateDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Starting price must be greater than 0")]
        public decimal StartingPrice { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }

    public class AuctionResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal StartingPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public AuctionStatus Status { get; set; }
        public string CreatedByUserId { get; set; } = string.Empty;
        public string CreatedByDisplayName { get; set; } = string.Empty;
        public int BidCount { get; set; }
        public bool IsActive { get; set; }
        public TimeSpan TimeRemaining { get; set; }
    }
}
