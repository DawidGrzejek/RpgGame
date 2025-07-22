using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RpgGame.Application.Behaviors;
using RpgGame.Application.Commands.Characters;
using RpgGame.Application.Commands.Combat;
using RpgGame.Application.Commands.Game;
using RpgGame.Application.Commands.Results;
using RpgGame.Application.Events;
using RpgGame.Application.Handlers.Events.Character;
using RpgGame.Application.Handlers.Events.Users;
using RpgGame.Application.Handlers.Users;
using RpgGame.Application.Interfaces.Services;
using RpgGame.Application.Services;
using RpgGame.Domain.Common;
using RpgGame.Domain.Events.Characters;
using RpgGame.Domain.Events.Users;
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
            services.AddScoped<IEventHandler<UserRegisteredEvent>, UserRegisteredEventHandler>();
            //services.AddScoped<IEventHandler<UserLoggedInEvent>, UserLoggedInEventHandler>();
            //services.AddScoped<IEventHandler<UserRoleAddedEvent>, UserRoleAddedEventHandler>();
            //services.AddScoped<IEventHandler<UserRoleRemovedEvent>, UserRoleRemovedEventHandler>();
            //services.AddScoped<IEventHandler<CharacterAssignedToUserEvent>, CharacterAssignedToUserEventHandler>();
            //services.AddScoped<IEventHandler<CharacterRemovedFromUserEvent>, CharacterRemovedFromUserEventHandler>();
            //services.AddScoped<IEventHandler<UserDeactivatedEvent>, UserDeactivatedEventHandler>();
            //services.AddScoped<IEventHandler<UserReactivatedEvent>, UserReactivatedEventHandler>();
            //services.AddScoped<IEventHandler<UserAchievementUnlockedEvent>, UserAchievementUnlockedEventHandler>();
            //services.AddScoped<IEventHandler<UserStatisticsUpdatedEvent>, UserStatisticsUpdatedEventHandler>();
            //services.AddScoped<IEventHandler<UserPreferencesUpdatedEvent>, UserPreferencesUpdatedEventHandler>();
            //services.AddScoped<IEventHandler<UserProfileUpdatedEvent>, UserProfileUpdatedEventHandler>();

        //services.AddScoped<IRequestHandler<PerformAttackCommand, OperationResult<CombatResult>>, PerformAttackCommandHandler>();
        services.AddScoped<IRequestHandler<FleeCombatCommand, OperationResult<FleeResult>>, FleeCombatCommandHandler>();
        services.AddScoped<IRequestHandler<MoveCharacterCommand, OperationResult<LocationChangeResult>>, MoveCharacterCommandHandler>();
        services.AddScoped<IRequestHandler<ExploreLocationCommand, OperationResult<ExploreResult>>, ExploreLocationCommandHandler>();
        services.AddScoped<IRequestHandler<ProcessCharacterDeathCommand, OperationResult<CharacterDeathResult>>, ProcessCharacterDeathCommandHandler>();

        // Register Application Services Interfaces
        // services.AddScoped<INotificationService, NotificationService>();
        // services.AddScoped<IEmailService, EmailService>();
        // services.AddScoped<IAnalyticsService, AnalyticsService>();
        // services.AddScoped<ISecurityService, SecurityService>();
        // services.AddScoped<IPermissionService, PermissionService>();
        // services.AddScoped<ISessionService, SessionService>();
        // services.AddScoped<IAuditService, AuditService>();
        // services.AddScoped<IAchievementService, AchievementService>();

        return services;
    }
}