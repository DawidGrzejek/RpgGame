using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RpgGame.Application.Behaviors;
using RpgGame.Application.Events;
using RpgGame.Application.Events.Handlers.Character;
using RpgGame.Domain.Events.Characters;
using System.Reflection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Register FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Register pipeline behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));

        // Register event infrastructure
        services.AddScoped<IEventDispatcher, EventDispatcher>();
        services.AddScoped<IEventSourcingService, EventSourcingService>();

        // Register event handlers
        services.AddScoped<IEventHandler<CharacterLeveledUp>, CharacterLeveledUpHandler>();
        services.AddScoped<IEventHandler<CharacterDied>, CharacterDiedHandler>();
        // Register other handlers

        return services;
    }
}