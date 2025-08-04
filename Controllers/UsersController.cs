using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using AuctionCommerce.Models;

namespace AuctionCommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Hämta alla användare (endast för Admin)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<object>> GetUsers()
        {
            var users = _userManager.Users.Select(u => new
            {
                u.Id,
                u.UserName,
                u.Email,
                u.IsEmailConfirmed,
                u.CreatedAt
            }).ToList();

            return Ok(users);
        }

        /// <summary>
        /// Hämta specifik användare
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.IsEmailConfirmed,
                user.CreatedAt,
                Roles = roles
            });
        }
    }
}
