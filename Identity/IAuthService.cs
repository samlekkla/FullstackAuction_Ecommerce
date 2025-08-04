using AuctionCommerce.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace AuctionCommerce.Identity
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(string email, string password, string displayName);
        Task<AppUser?> LoginAsync(string email, string password);
        Task<string> GenerateJwtTokenAsync(AppUser user);
        Task<AppUser?> GetUserByIdAsync(string userId);
        Task<AppUser?> GetUserByEmailAsync(string email);
        Task<IdentityResult> UpdateUserAsync(AppUser user);
        Task<IdentityResult> ChangePasswordAsync(AppUser user, string currentPassword, string newPassword);
        Task<bool> IsEmailTakenAsync(string email);
    }
}
