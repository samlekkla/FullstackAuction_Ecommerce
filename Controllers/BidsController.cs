using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AuctionCommerce.Data.Entities;
using AuctionCommerce.Core.Services;
using AuctionCommerce.DTOs;
using System.Security.Claims;

namespace AuctionCommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BidsController : ControllerBase
    {
        private readonly IBidService _bidService;

        public BidsController(IBidService bidService)
        {
            _bidService = bidService;
        }

        // GET: api/Bids/auction/5
        [HttpGet("auction/{auctionId}")]
        public async Task<ActionResult<IEnumerable<BidResponseDto>>> GetBidsByAuction(int auctionId)
        {
            var bids = await _bidService.GetBidsByAuctionAsync(auctionId);
            var bidDtos = bids.Select(MapToResponseDto);
            return Ok(bidDtos);
        }

        // GET: api/Bids/user/me
        [HttpGet("user/me")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<BidResponseDto>>> GetMyBids()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var bids = await _bidService.GetBidsByUserAsync(userId);
            var bidDtos = bids.Select(MapToResponseDto);
            return Ok(bidDtos);
        }

        // POST: api/Bids
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<BidResponseDto>> PlaceBid(BidCreateDto bidDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Check if user can place this bid
            var canBid = await _bidService.CanPlaceBidAsync(bidDto.AuctionId, userId, bidDto.Amount);
            if (!canBid)
            {
                return BadRequest("Cannot place this bid");
            }

            var bid = new Bid
            {
                AuctionId = bidDto.AuctionId,
                UserId = userId,
                Amount = bidDto.Amount
            };

            try
            {
                var placedBid = await _bidService.PlaceBidAsync(bid);
                return CreatedAtAction(nameof(GetBidsByAuction), 
                    new { auctionId = placedBid.AuctionId }, 
                    MapToResponseDto(placedBid));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Bids/user/{userId}/winning
        [HttpGet("user/{userId}/winning")]
        public async Task<ActionResult<IEnumerable<BidResponseDto>>> GetWinningBidsForUser(string userId)
        {
            var bids = await _bidService.GetWinningBidsForUserAsync(userId);
            var bidDtos = bids.Select(MapToResponseDto);
            return Ok(bidDtos);
        }

        private static BidResponseDto MapToResponseDto(Bid bid)
        {
            return new BidResponseDto
            {
                Id = bid.Id,
                Amount = bid.Amount,
                CreatedAt = bid.CreatedAt,
                Status = bid.Status,
                AuctionId = bid.AuctionId,
                AuctionTitle = bid.Auction?.Title ?? "Unknown",
                UserId = bid.UserId,
                UserDisplayName = bid.User?.DisplayName ?? "Unknown",
                IsHighestBid = false // This would need to be calculated based on other bids
            };
        }
    }
}
