using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RpgGame.Application.Events;
using RpgGame.Application.Repositories;
using RpgGame.Domain.Interfaces.Repositories;
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
            services.AddDbContext<GameDbContext>();

            // Add ASP.NET Core Identity services
            services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            // Register repositories - this is where Infrastructure implementations are connected to Application interfaces

            // Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register event store repository - implement Application interface with Infrastructure implementation
            services.AddScoped<IEventStoreRepository, EventStoreRepository>();

            // Register domain repositories - implement Application interfaces with Infrastructure implementations
            services.AddScoped<ICharacterRepository, CharacterRepository>();
            services.AddScoped<IGameSaveRepository, GameSaveRepository>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<IEnemyTemplateRepository, EnemyTemplateRepository>();
            services.AddScoped<IItemTemplateRepository, ItemTemplateRepository>();

            return services;
        }
    }
}