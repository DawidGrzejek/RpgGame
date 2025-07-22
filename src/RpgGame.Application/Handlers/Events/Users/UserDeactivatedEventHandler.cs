using Microsoft.Extensions.Logging;
using RpgGame.Application.Events;
using RpgGame.Application.Interfaces.Persistence;
using RpgGame.Application.Interfaces.Repositories;
using RpgGame.Application.Interfaces.Services;
using RpgGame.Domain.Events.Users;
namespace RpgGame.Application.Handlers.Events.Users
{
/// <summary>
    /// Handles UserDeactivatedEvent - performs cleanup and audit logging
    /// </summary>
    public class UserDeactivatedEventHandler : IEventHandler<UserDeactivatedEvent>
    {
        private readonly ILogger<UserDeactivatedEventHandler> _logger;
        // private readonly INotificationService _notificationService;
        private readonly ISessionService _sessionService;
        private readonly IAuditService _auditService;

        public UserDeactivatedEventHandler(
            ILogger<UserDeactivatedEventHandler> logger,
            // INotificationService notificationService,
            ISessionService sessionService,
            IAuditService auditService)
        {
            _logger = logger;
            // _notificationService = notificationService;
            _sessionService = sessionService;
            _auditService = auditService;
        }

        public async Task HandleAsync(UserDeactivatedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning("User {Username} deactivated by {DeactivatedBy}. Reason: {Reason}", 
                domainEvent.Username, domainEvent.DeactivatedBy, domainEvent.DeactivationReason);

            try
            {
                // Terminate all active sessions
                await _sessionService.TerminateAllUserSessionsAsync(domainEvent.AggregateId, cancellationToken);

                // Send deactivation notification (if not a ban)
                if (!domainEvent.DeactivationReason.Contains("ban", StringComparison.OrdinalIgnoreCase))
                {
                    var message = domainEvent.IsTemporary 
                        ? $"Your account has been temporarily deactivated until {domainEvent.ReactivationDate:yyyy-MM-dd}. Reason: {domainEvent.DeactivationReason}"
                        : $"Your account has been deactivated. Reason: {domainEvent.DeactivationReason}";

                    // await _notificationService.SendNotificationAsync(
                    //     domainEvent.AggregateId,
                    //     "Account Deactivated",
                    //     message,
                    //     cancellationToken);
                }

                // Create audit log entry
                await _auditService.LogUserActionAsync(
                    domainEvent.DeactivatedBy,
                    "USER_DEACTIVATED",
                    $"Deactivated user {domainEvent.Username}",
                    new { 
                        TargetUserId = domainEvent.AggregateId,
                        TargetUsername = domainEvent.Username,
                        Reason = domainEvent.DeactivationReason,
                        IsTemporary = domainEvent.IsTemporary,
                        ReactivationDate = domainEvent.ReactivationDate
                    },
                    cancellationToken);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling UserDeactivatedEvent for user {Username}", domainEvent.Username);
            }
        }
    }
}