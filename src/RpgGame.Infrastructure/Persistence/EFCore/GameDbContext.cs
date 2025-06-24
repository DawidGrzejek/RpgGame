using Microsoft.EntityFrameworkCore;
using RpgGame.Domain.Events.Base;
using RpgGame.Infrastructure.Persistence.EFCore.Configurations;
using Microsoft.Extensions.Configuration;

namespace RpgGame.Infrastructure.Persistence.EFCore
{
    public class GameDbContext : DbContext
    {
        public DbSet<GameSave> GameSaves { get; set; }
        public DbSet<StoredEvent> Events { get; set; }

        // Default constructor for use without explicit options
        public GameDbContext() : base()
        {
        }

        // Constructor that accepts options for dependency injection and testing
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
        {
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

                optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorCodesToAdd: null);
                    npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    npgsqlOptions.CommandTimeout(60);
                    npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "RpgGame");
                })
                .UseEnumCheckConstraints()
                .UseSnakeCaseNamingConvention();
                
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    optionsBuilder.EnableDetailedErrors(); // Enable detailed errors in development
                    optionsBuilder.EnableSensitiveDataLogging(); // Enable sensitive data logging in development
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("RpgGame");
            modelBuilder.ApplyConfiguration(new GameSaveConfiguration());
            modelBuilder.ApplyConfiguration(new StoredEventConfiguration());
        }
    }
}