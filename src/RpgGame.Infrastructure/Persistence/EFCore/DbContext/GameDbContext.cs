using Microsoft.EntityFrameworkCore;
using RpgGame.Domain.Events.Base;
using RpgGame.Infrastructure.Persistence.EFCore.Configurations;
using Microsoft.Extensions.Configuration;
using RpgGame.Domain.Entities.Users;
using RpgGame.Infrastructure.Persistence.Entities;

namespace RpgGame.Infrastructure.Persistence.EFCore
{
    public class GameDbContext : DbContext
    {
        public DbSet<GameSave> GameSaves { get; set; }
        public DbSet<StoredEvent> Events { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        // Default constructor for use without explicit options
        public GameDbContext() : base()
        {
        }

        // Constructor that accepts options for dependency injection and testing
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("RpgGame");
            modelBuilder.ApplyConfiguration(new GameSaveConfiguration());
            modelBuilder.ApplyConfiguration(new StoredEventConfiguration());
            //modelBuilder.ApplyConfiguration(new CharacterConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
            
            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.AspNetUserId).IsRequired();
                entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Roles)
                    .HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                    );
                entity.OwnsOne(u => u.Preferences);
            });

            // RefreshToken configuration
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.Id);
                entity.Property(rt => rt.Token).IsRequired();
                entity.Property(rt => rt.UserId).IsRequired();
                entity.HasIndex(rt => rt.Token).IsUnique();
            });
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
    }
}