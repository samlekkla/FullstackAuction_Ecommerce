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
    public class AuctionsController : ControllerBase
    {
        private readonly IAuctionService _auctionService;

        public AuctionsController(IAuctionService auctionService)
        {
            _auctionService = auctionService;
        }

        // GET: api/Auctions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuctionResponseDto>>> GetAuctions()
        {
            var auctions = await _auctionService.GetActiveAuctionsAsync();
            var auctionDtos = auctions.Select(MapToResponseDto);
            return Ok(auctionDtos);
        }

        // GET: api/Auctions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionResponseDto>> GetAuction(int id)
        {
            var auction = await _auctionService.GetAuctionByIdAsync(id);
            
            if (auction == null)
            {
                return NotFound();
            }

            return Ok(MapToResponseDto(auction));
        }

        // POST: api/Auctions
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<AuctionResponseDto>> PostAuction(AuctionCreateDto auctionDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var auction = new Auction
            {
                Title = auctionDto.Title,
                Description = auctionDto.Description,
                StartingPrice = auctionDto.StartingPrice,
                StartDate = auctionDto.StartDate,
                EndDate = auctionDto.EndDate,
                CreatedByUserId = userId
            };

            var createdAuction = await _auctionService.CreateAuctionAsync(auction);
            return CreatedAtAction(nameof(GetAuction), new { id = createdAuction.Id }, MapToResponseDto(createdAuction));
        }

        // PUT: api/Auctions/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutAuction(int id, AuctionUpdateDto auctionDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var existingAuction = await _auctionService.GetAuctionByIdAsync(id);
            if (existingAuction == null)
            {
                return NotFound();
            }

            if (existingAuction.CreatedByUserId != userId)
            {
                return Forbid();
            }

            var auction = new Auction
            {
                Title = auctionDto.Title,
                Description = auctionDto.Description,
                StartingPrice = auctionDto.StartingPrice,
                StartDate = auctionDto.StartDate,
                EndDate = auctionDto.EndDate
            };

            try
            {
                await _auctionService.UpdateAuctionAsync(id, auction);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Auctions/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAuction(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var auction = await _auctionService.GetAuctionByIdAsync(id);
            if (auction == null)
            {
                return NotFound();
            }

            if (auction.CreatedByUserId != userId)
            {
                return Forbid();
            }

            try
            {
                await _auctionService.DeleteAuctionAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Auctions/search?term=searchTerm
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<AuctionResponseDto>>> SearchAuctions([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return BadRequest("Search term is required");
            }

            var auctions = await _auctionService.SearchAuctionsAsync(term);
            var auctionDtos = auctions.Select(MapToResponseDto);
            return Ok(auctionDtos);
        }

        // GET: api/Auctions/ending-soon
        [HttpGet("ending-soon")]
        public async Task<ActionResult<IEnumerable<AuctionResponseDto>>> GetEndingSoon([FromQuery] int hours = 24)
        {
            var auctions = await _auctionService.GetEndingSoonAsync(hours);
            var auctionDtos = auctions.Select(MapToResponseDto);
            return Ok(auctionDtos);
        }

        private static AuctionResponseDto MapToResponseDto(Auction auction)
        {
            return new AuctionResponseDto
            {
                Id = auction.Id,
                Title = auction.Title,
                Description = auction.Description,
                StartingPrice = auction.StartingPrice,
                CurrentPrice = auction.Bids?.Any() == true ? auction.Bids.Max(b => b.Amount) : auction.StartingPrice,
                StartDate = auction.StartDate,
                EndDate = auction.EndDate,
                CreatedAt = auction.CreatedAt,
                Status = auction.Status,
                CreatedByUserId = auction.CreatedByUserId,
                CreatedByDisplayName = auction.CreatedBy?.DisplayName ?? "Unknown",
                BidCount = auction.Bids?.Count ?? 0,
                IsActive = auction.Status == Data.Enums.AuctionStatus.Active && 
                          auction.StartDate <= DateTime.UtcNow && 
                          auction.EndDate > DateTime.UtcNow,
                TimeRemaining = auction.EndDate > DateTime.UtcNow ? 
                               auction.EndDate - DateTime.UtcNow : 
                               TimeSpan.Zero
            };
        }
    }
}
