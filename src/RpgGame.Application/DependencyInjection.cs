using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RpgGame.Application.Behaviors;
using RpgGame.Application.Commands.Characters;
using RpgGame.Application.Commands.Combat;
using RpgGame.Application.Commands.Game;
using RpgGame.Application.Commands.Results;
using RpgGame.Application.Events;
using RpgGame.Application.Events.Handlers.Character;
using RpgGame.Application.Interfaces.Services;
using RpgGame.Application.Repositories;
using RpgGame.Application.Services;
using RpgGame.Domain.Common;
using RpgGame.Domain.Events.Characters;
using System.Reflection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Register FluentValidation
        //services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssemblyContaining<PerformAttackCommandValidator>();

        // Register GameSaveService as scoped
        services.AddScoped<IGameSaveService, GameSaveService>();

        // Register IQuestService
        services.AddScoped<IQuestService, QuestService>();
        services.AddScoped<ICharacterService, CharacterService>();

        // Register pipeline behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        // Register event infrastructure
        services.AddScoped<IEventDispatcher, EventDispatcher>();
        services.AddScoped<IEventSourcingService, EventSourcingService>();

        // Register event handlers
        services.AddScoped<IEventHandler<CharacterLeveledUp>, CharacterLeveledUpHandler>();
        services.AddScoped<IEventHandler<CharacterDied>, CharacterDiedHandler>();

        //services.AddScoped<IRequestHandler<PerformAttackCommand, OperationResult<CombatResult>>, PerformAttackCommandHandler>();
        services.AddScoped<IRequestHandler<FleeCombatCommand, OperationResult<FleeResult>>, FleeCombatCommandHandler>();
        services.AddScoped<IRequestHandler<MoveCharacterCommand, OperationResult<LocationChangeResult>>, MoveCharacterCommandHandler>();
        services.AddScoped<IRequestHandler<ExploreLocationCommand, OperationResult<ExploreResult>>, ExploreLocationCommandHandler>();
        services.AddScoped<IRequestHandler<ProcessCharacterDeathCommand, OperationResult<CharacterDeathResult>>, ProcessCharacterDeathCommandHandler>();

        return services;
    }
}