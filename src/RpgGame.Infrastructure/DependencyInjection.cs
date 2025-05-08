using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RpgGame.Application.Events;
using RpgGame.Application.Repositories;
using RpgGame.Infrastructure.EventStore;
using RpgGame.Infrastructure.Persistence.EFCore;
using RpgGame.Infrastructure.Persistence.Repositories;

namespace RpgGame.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Your existing database context registration
            services.AddDbContext<GameDbContext>();

            // Register event store repository
            services.AddScoped<IEventStoreRepository, EventStoreRepository>();

            services.AddScoped<ICharacterRepository, CharacterRepository>();

            return services;
        }
    }
}
