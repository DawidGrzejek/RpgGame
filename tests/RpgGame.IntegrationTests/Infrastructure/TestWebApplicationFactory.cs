using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RpgGame.Infrastructure.Persistence.EFCore;

namespace RpgGame.IntegrationTests.Infrastructure
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the real database context
                services.RemoveAll(typeof(DbContextOptions<GameDbContext>));
                services.RemoveAll(typeof(GameDbContext));

                // Add in-memory database for testing
                services.AddDbContext<GameDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                });

                // Replace the Identity database context with in-memory version
                services.RemoveAll(typeof(DbContextOptions<IdentityDataContext>));
                services.AddDbContext<IdentityDataContext>(options =>
                {
                    options.UseInMemoryDatabase("TestIdentityDatabase");
                });

                // Configure Identity options for testing (don't re-register Identity services)
                services.Configure<IdentityOptions>(options =>
                {
                    // Relaxed password requirements for testing
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 3;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    
                    // Disable email confirmation for testing
                    options.User.RequireUniqueEmail = true;
                    options.SignIn.RequireConfirmedEmail = false;
                });
                
                // Override other services if needed for testing
                // For example, you might want to mock external APIs, email services, etc.
            });

            builder.UseEnvironment("Testing");
        }

        /// <summary>
        /// Get a fresh database context for verification in tests
        /// </summary>
        public GameDbContext GetDbContext()
        {
            var scope = Services.CreateScope();
            return scope.ServiceProvider.GetRequiredService<GameDbContext>();
        }

        /// <summary>
        /// Clean the database between tests
        /// </summary>
        public async Task ResetDatabaseAsync()
        {
            using var scope = Services.CreateScope();
            
            // Clear game database
            var gameContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();
            if (gameContext.Users.Any())
            {
                gameContext.Users.RemoveRange(gameContext.Users);
            }
            await gameContext.SaveChangesAsync();
            
            // Clear identity database
            var identityContext = scope.ServiceProvider.GetRequiredService<IdentityDataContext>();
            if (identityContext.Users.Any())
            {
                identityContext.Users.RemoveRange(identityContext.Users);
            }
            await identityContext.SaveChangesAsync();
        }
    }
}