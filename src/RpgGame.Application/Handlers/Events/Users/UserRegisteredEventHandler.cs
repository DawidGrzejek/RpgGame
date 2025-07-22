using Microsoft.Extensions.Logging;
using RpgGame.Application.Events;
using RpgGame.Application.Interfaces.Persistence;
using RpgGame.Application.Interfaces.Repositories;
using RpgGame.Application.Interfaces.Services;
using RpgGame.Domain.Events.Users;

namespace RpgGame.Application.Handlers.Users
{
    /// <summary>
    /// Handles UserRegisteredEvent - performs welcome actions for new users
    /// </summary>
    public class UserRegisteredEventHandler : IEventHandler<UserRegisteredEvent>
    {
        private readonly ILogger<UserRegisteredEventHandler> _logger;
        // private readonly INotificationService _notificationService;
        // private readonly IEmailService _emailService;

        public UserRegisteredEventHandler(
            ILogger<UserRegisteredEventHandler> logger
            // INotificationService notificationService,
            // IEmailService emailService
            )
        {
            _logger = logger;
            // _notificationService = notificationService;
            // _emailService = emailService;
        }

        public async Task HandleAsync(UserRegisteredEvent domainEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Handling UserRegisteredEvent for user {Username} (ID: {UserId})", 
                domainEvent.Username, domainEvent.AggregateId);

            try
            {
                // Send welcome email
                // await _emailService.SendWelcomeEmailAsync(domainEvent.Email, domainEvent.Username, cancellationToken);

                // Send in-app notification
                // await _notificationService.SendNotificationAsync(
                //     domainEvent.AggregateId,
                //     "Welcome to RPG Game!",
                //     "Welcome to our RPG adventure! Start by creating your first character.",
                //     cancellationToken);

                // Log analytics event
                _logger.LogInformation("New user registered: {Username} at {RegistrationDate}", 
                    domainEvent.Username, domainEvent.RegistrationDate);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling UserRegisteredEvent for user {Username}", domainEvent.Username);
                // Don't rethrow - registration should succeed even if welcome actions fail
            }
        }
    }
}