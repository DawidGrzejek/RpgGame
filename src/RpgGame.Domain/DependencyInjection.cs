using Microsoft.Extensions.DependencyInjection;
using RpgGame.Domain.Entities.World;
using RpgGame.Domain.Interfaces.World;

namespace RpgGame.Domain
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            // Register GameWorld as singleton
            services.AddSingleton<GameWorld>();
            services.AddSingleton<IGameWorld>(provider => provider.GetRequiredService<GameWorld>());

            return services;
        }
    }
}