using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace RpgGame.Infrastructure.Persistence.EFCore
{
    public class IdentityDataContext : IdentityDbContext<IdentityUser>
    {
        public IdentityDataContext(DbContextOptions<IdentityDataContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("AspNetIdentity");
        }

        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .AddEnvironmentVariables()
                    .Build();

                // Get connection string from environment variable or configuration
                var connectionString = Environment.GetEnvironmentVariable("RPG_GAME_DB_CONNECTION_STRING", EnvironmentVariableTarget.Machine) ??
                                       configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException(
                        "DefaultConnection string is required in configuration. " +
                        "Please ensure your appsettings.json contains a valid PostgreSQL connection string.");
                }

                optionsBuilder.UseNpgsql(connectionString);
                
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    optionsBuilder.EnableDetailedErrors(); // Enable detailed errors in development
                    optionsBuilder.EnableSensitiveDataLogging(); // Enable sensitive data logging in development
                }
            }
        }
    }
}