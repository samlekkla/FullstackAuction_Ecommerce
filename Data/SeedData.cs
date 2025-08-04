using Microsoft.AspNetCore.Identity;
using AuctionCommerce.Data.Entities;

namespace AuctionCommerce.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                await CreateRolesAsync(roleManager, logger);
                await CreateAdminUserAsync(userManager, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Fel vid seed av data");
                throw;
            }
        }

        private static async Task CreateRolesAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
        {
            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    if (result.Succeeded)
                    {
                        logger.LogInformation($"Roll '{role}' skapad framgångsrikt");
                    }
                    else
                    {
                        logger.LogError($"Kunde inte skapa roll '{role}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    logger.LogInformation($"Roll '{role}' finns redan");
                }
            }
        }

        private static async Task CreateAdminUserAsync(UserManager<AppUser> userManager, ILogger logger)
        {
            const string adminEmail = "admin@auctioncommerce.com";
            const string adminPassword = "Admin123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    DisplayName = "Administrator",
                    IsEmailVerified = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    logger.LogInformation($"Admin-användare '{adminEmail}' skapad framgångsrikt");
                }
                else
                {
                    logger.LogError($"Kunde inte skapa admin-användare: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                logger.LogInformation($"Admin-användare '{adminEmail}' finns redan");
            }
        }
    }
}
