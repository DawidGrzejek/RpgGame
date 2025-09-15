using Microsoft.EntityFrameworkCore;
using RpgGame.Domain.Events.Base;
using RpgGame.Infrastructure.Persistence.EFCore.Configurations;
using Microsoft.Extensions.Configuration;
using RpgGame.Domain.Entities.Users;
using RpgGame.Domain.Entities.Configuration;
using RpgGame.Domain.Entities.EventSourcing;
using RpgGame.Infrastructure.Persistence.Entities;

namespace RpgGame.Infrastructure.Persistence.EFCore
{
    public class GameDbContext : DbContext
    {
        public DbSet<GameSave> GameSaves { get; set; }
        public DbSet<StoredEvent> Events { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<ItemTemplate> ItemTemplates { get; set; }
        public DbSet<CharacterTemplate> CharacterTemplates { get; set; }
        public DbSet<AbilityTemplate> AbilityTemplates { get; set; }
        public DbSet<CharacterSnapshot> CharacterSnapshots { get; set; }

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
            
            // Configure all DateTime properties to use datetime2 by default
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetColumnType("datetime2");
                    }
                }
            }
            
            modelBuilder.ApplyConfiguration(new GameSaveConfiguration());
            modelBuilder.ApplyConfiguration(new StoredEventConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
            modelBuilder.ApplyConfiguration(new ItemTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new CharacterTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new AbilityTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new CharacterSnapshotConfiguration());
            
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
                
                // Configure DateTime properties to use UTC and datetime2 SQL type
                entity.Property(u => u.CreatedAt)
                    .HasColumnType("datetime2")
                    .HasConversion(
                        v => v.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v, DateTimeKind.Utc) : v.ToUniversalTime(),
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
                
                entity.Property(u => u.LastLoginAt)
                    .HasColumnType("datetime2")
                    .HasConversion(
                        v => v.HasValue 
                            ? (v.Value.Kind == DateTimeKind.Unspecified 
                                ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) 
                                : v.Value.ToUniversalTime()) 
                            : (DateTime?)null,
                        v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null);
                
                entity.OwnsOne(u => u.Preferences);
                entity.OwnsOne(u => u.Statistics, stats =>
                {
                    stats.Property(s => s.FirstLoginDate)
                        .HasColumnType("datetime2")
                        .HasConversion(
                            v => v.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v, DateTimeKind.Utc) : v.ToUniversalTime(),
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
                    
                    stats.Property(s => s.LastActiveDate)
                        .HasColumnType("datetime2")
                        .HasConversion(
                            v => v.HasValue 
                                ? (v.Value.Kind == DateTimeKind.Unspecified 
                                    ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) 
                                    : v.Value.ToUniversalTime()) 
                                : (DateTime?)null,
                            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null);
                });
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

                // Get connection string from configuration only (temporarily disable env var)
                var connectionString = configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException(
                        "DefaultConnection string is required in configuration. " +
                        "Please ensure your appsettings.json contains a valid SQL Server connection string.");
                }

                optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                    sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    sqlOptions.CommandTimeout(60);
                    sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "RpgGame");
                });
                
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    optionsBuilder.EnableDetailedErrors(); // Enable detailed errors in development
                    optionsBuilder.EnableSensitiveDataLogging(); // Enable sensitive data logging in development
                }
            }
        }
    }
}