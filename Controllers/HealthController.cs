using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AuctionCommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Health check endpoint för att testa CORS och API-tillgänglighet
        /// </summary>
        [HttpGet]
        public ActionResult<object> GetHealth()
        {
            var origin = Request.Headers["Origin"].ToString();
            var userAgent = Request.Headers["User-Agent"].ToString();
            
            _logger.LogInformation("Health check requested from origin: {Origin}, User-Agent: {UserAgent}", origin, userAgent);

            return Ok(new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Origin = string.IsNullOrEmpty(origin) ? "Direct API call" : origin,
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                Message = "Auction Commerce API is running successfully",
                CorsEnabled = true,
                Version = "1.0.0"
            });
        }

        /// <summary>
        /// Explicit OPTIONS endpoint för CORS preflight testing
        /// </summary>
        [HttpOptions]
        public ActionResult Options()
        {
            var origin = Request.Headers["Origin"].ToString();
            _logger.LogInformation("OPTIONS preflight request from origin: {Origin}", origin);
            
            return Ok(new { Message = "CORS preflight handled successfully" });
        }

        /// <summary>
        /// Test endpoint för att verifiera CORS med olika HTTP methods
        /// </summary>
        [HttpPost("test")]
        public ActionResult<object> PostTest([FromBody] object testData)
        {
            var origin = Request.Headers["Origin"].ToString();
            _logger.LogInformation("POST test requested from origin: {Origin}", origin);

            return Ok(new
            {
                Status = "POST request successful",
                Origin = origin,
                ReceivedData = testData,
                Timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Secure endpoint för att testa CORS med autentisering
        /// </summary>
        [HttpGet("secure")]
        [Authorize]
        public ActionResult<object> GetSecureHealth()
        {
            var origin = Request.Headers["Origin"].ToString();
            var userId = User.Identity?.Name;
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

            return Ok(new
            {
                Status = "Authenticated and healthy",
                Timestamp = DateTime.UtcNow,
                Origin = origin,
                UserId = userId,
                Claims = claims,
                Message = "Secure endpoint accessible with CORS"
            });
        }

        /// <summary>
        /// CORS information endpoint
        /// </summary>
        [HttpGet("cors-info")]
        public ActionResult<object> GetCorsInfo()
        {
            var headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            
            return Ok(new
            {
                RequestHeaders = headers,
                AllowedOrigins = "Development: All origins allowed",
                AllowedMethods = "GET, POST, PUT, DELETE, PATCH, OPTIONS",
                AllowedHeaders = "All headers",
                SupportsCredentials = true,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
