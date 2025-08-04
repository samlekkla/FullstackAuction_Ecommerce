using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AuctionCommerce.DTOs;
using AuctionCommerce.Data.Entities;
using AuctionCommerce.Middleware;

namespace AuctionCommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IJwtService jwtService,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _logger = logger;
        }

        /// <summary>
        /// Registrera en ny användare
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existingUser != null)
                {
                    return BadRequest(new { message = "En användare med denna e-postadress finns redan." });
                }

                var user = new AppUser
                {
                    UserName = registerDto.Email,
                    Email = registerDto.Email,
                    DisplayName = registerDto.DisplayName,
                    IsEmailVerified = false,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, registerDto.Password);

                if (!result.Succeeded)
                {
                    return BadRequest(new { message = "Kunde inte skapa användare.", errors = result.Errors });
                }

                // Lägg till användaren i "User" rollen
                await _userManager.AddToRoleAsync(user, "User");

                // Generera JWT token
                var roles = await _userManager.GetRolesAsync(user);
                var token = _jwtService.GenerateToken(user.Id, user.Email!, user.DisplayName, roles);

                return Ok(new AuthResponseDto
                {
                    Token = token,
                    Email = user.Email!,
                    UserId = user.Id,
                    Roles = roles.ToList(),
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fel vid registrering av användare");
                return StatusCode(500, new { message = "Ett internt fel uppstod." });
            }
        }

        /// <summary>
        /// Logga in en användare
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user == null)
                {
                    return Unauthorized(new { message = "Ogiltiga inloggningsuppgifter." });
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
                if (!result.Succeeded)
                {
                    return Unauthorized(new { message = "Ogiltiga inloggningsuppgifter." });
                }

                // Generera JWT token
                var roles = await _userManager.GetRolesAsync(user);
                var token = _jwtService.GenerateToken(user.Id, user.Email!, user.DisplayName, roles);

                return Ok(new AuthResponseDto
                {
                    Token = token,
                    Email = user.Email!,
                    UserId = user.Id,
                    DisplayName = user.DisplayName,
                    Roles = roles.ToList(),
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fel vid inloggning");
                return StatusCode(500, new { message = "Ett internt fel uppstod." });
            }
        }

        /// <summary>
        /// Hämta information om den inloggade användaren
        /// </summary>
        [HttpGet("me")]
        public async Task<ActionResult<object>> GetCurrentUser()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                user.Id,
                user.Email,
                user.DisplayName,
                user.IsEmailVerified,
                user.CreatedAt,
                Roles = roles
            });
        }
    }
}
