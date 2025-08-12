using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RpgGame.Application.Interfaces.Repositories;
using RpgGame.Domain.Entities.Users;

namespace RpgGame.Infrastructure.Services
{
    public class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<DatabaseSeeder>>();

            try
            {
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var userRepository = services.GetRequiredService<IUserRepository>();

                await SeedRolesAsync(roleManager, logger);
                await SeedUsersAsync(userManager, userRepository, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
        {
            var roles = new[]
            {
                "GameMaster",
                "Admin", 
                "Moderator",
                "Player"
            };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new IdentityRole(roleName);
                    var result = await roleManager.CreateAsync(role);
                    
                    if (result.Succeeded)
                    {
                        logger.LogInformation("Created role: {RoleName}", roleName);
                    }
                    else
                    {
                        logger.LogError("Failed to create role {RoleName}: {Errors}", 
                            roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    logger.LogInformation("Role {RoleName} already exists", roleName);
                }
            }
        }

        private static async Task SeedUsersAsync(UserManager<IdentityUser> userManager, IUserRepository userRepository, ILogger logger)
        {
            // Create default GameMaster user
            const string gameMasterEmail = "gamemaster@rpggame.local";
            const string gameMasterUsername = "GameMaster";
            var gameMasterPassword = Environment.GetEnvironmentVariable("SEED_USER_PASSWORD") ?? throw new InvalidOperationException("SEED_USER_PASSWORD environment variable is not set");

            var existingUser = await userManager.FindByEmailAsync(gameMasterEmail);
            if (existingUser == null)
            {
                var gameMasterIdentityUser = new IdentityUser
                {
                    UserName = gameMasterUsername,
                    Email = gameMasterEmail,
                    EmailConfirmed = true,
                    LockoutEnabled = false
                };

                var result = await userManager.CreateAsync(gameMasterIdentityUser, gameMasterPassword);
                if (result.Succeeded)
                {
                    // Add to GameMaster and Admin roles
                    await userManager.AddToRoleAsync(gameMasterIdentityUser, "GameMaster");
                    await userManager.AddToRoleAsync(gameMasterIdentityUser, "Admin");

                    // Create corresponding User entity in our domain using factory method
                    var domainUser = User.Create(gameMasterIdentityUser.Id, gameMasterUsername, gameMasterEmail);
                    
                    // Add roles to the domain user
                    domainUser.AddRole("GameMaster");
                    domainUser.AddRole("Admin");

                    await userRepository.AddAsync(domainUser);
                    logger.LogInformation("Created GameMaster user: {Email}", gameMasterEmail);
                }
                else
                {
                    logger.LogError("Failed to create GameMaster user: {Errors}", 
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                logger.LogInformation("GameMaster user already exists: {Email}", gameMasterEmail);
            }
        }
    }
}