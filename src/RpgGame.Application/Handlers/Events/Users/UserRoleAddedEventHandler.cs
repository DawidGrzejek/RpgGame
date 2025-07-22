using Microsoft.Extensions.Logging;
using RpgGame.Application.Events;
using RpgGame.Application.Interfaces.Persistence;
using RpgGame.Application.Interfaces.Repositories;
using RpgGame.Application.Interfaces.Services;
using RpgGame.Domain.Events.Users;

namespace RpgGame.Application.Handlers.Events.Users
{

    /// <summary>
    /// Handles UserRoleAddedEvent - manages role-based permissions and notifications
    /// </summary>
    public class UserRoleAddedEventHandler : IEventHandler<UserRoleAddedEvent>
    {
        private readonly ILogger<UserRoleAddedEventHandler> _logger;
        private readonly IPermissionService _permissionService;
        // private readonly INotificationService _notificationService;

        public UserRoleAddedEventHandler(
            ILogger<UserRoleAddedEventHandler> logger,
            IPermissionService permissionService
            // INotificationService notificationService
            )
        {
            _logger = logger;
            _permissionService = permissionService;
            // _notificationService = notificationService;
        }

        public async Task HandleAsync(UserRoleAddedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Role {Role} added to user {Username} by {AddedBy}", 
                domainEvent.Role, domainEvent.Username, domainEvent.AddedBy);

            try
            {
                // Update user permissions cache
                await _permissionService.RefreshUserPermissionsAsync(domainEvent.AggregateId, cancellationToken);

                // Send role assignment notification
                var roleMessage = domainEvent.Role switch
                {
                    "Admin" => "You have been granted administrator privileges!",
                    "Moderator" => "You are now a community moderator!",
                    "VIP" => "Welcome to VIP status! Enjoy your exclusive benefits.",
                    _ => $"You have been assigned the {domainEvent.Role} role."
                };

                // await _notificationService.SendNotificationAsync(
                //     domainEvent.AggregateId,
                //     "New Role Assigned",
                //     roleMessage,
                //     cancellationToken);

                // Log audit trail
                _logger.LogInformation("Role assignment completed: User {Username} granted {Role} role by {AddedBy} at {Timestamp}",
                    domainEvent.Username, domainEvent.Role, domainEvent.AddedBy, domainEvent.RoleAssignedAt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling UserRoleAddedEvent for user {Username}", domainEvent.Username);
            }
        }
    }
}