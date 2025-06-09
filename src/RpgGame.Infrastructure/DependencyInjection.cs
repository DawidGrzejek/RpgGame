using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RpgGame.Application.Events;
using RpgGame.Application.Repositories;
using RpgGame.Infrastructure.EventStore;
using RpgGame.Infrastructure.Persistence.EFCore;
using RpgGame.Infrastructure.Persistence.Repositories;
using RpgGame.Infrastructure.Persistence.UnitOfWork;

namespace RpgGame.Infrastructure
{
    /// <summary>
    /// Extension methods for registering Infrastructure services in the DI container
    /// This is where Infrastructure layer implementations are registered for Application layer interfaces
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<GameDbContext>(options =>
            {
                // Get connection string from configuration
                var connectionString = configuration.GetConnectionString("DefaultConnection");

                // If connection string is provided, use it; otherwise, use default SQLite
                if (!string.IsNullOrEmpty(connectionString))
                {
                    // Check if the connection string indicates SQLite or SQL Server
                    if (connectionString.ToLower().Contains("data source=") ||
                        connectionString.ToLower().Contains(".db"))
                    {
                        options.UseSqlite(connectionString);
                    }
                    else
                    {
                        // Assume SQL Server if not SQLite
                        //options.UseSqlServer(connectionString);
                    }
                }
                else
                {
                    // Default to SQLite with default location
                    options.UseSqlite("Data Source=Data/rpggame.db");
                }
            });

            // Register repositories - this is where Infrastructure implementations are connected to Application interfaces

            // 1. Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // 2. Register event store repository - implement Application interface with Infrastructure implementation
            services.AddScoped<IEventStoreRepository, EventStoreRepository>();

            // 3. Register domain repositories - implement Application interfaces with Infrastructure implementations
            services.AddScoped<ICharacterRepository, CharacterRepository>();
            services.AddScoped<IGameSaveRepository, GameSaveRepository>();

            return services;
        }
    }
}