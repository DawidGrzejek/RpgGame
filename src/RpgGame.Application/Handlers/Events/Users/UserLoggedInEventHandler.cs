using Microsoft.Extensions.Logging;
using RpgGame.Application.Events;
using RpgGame.Application.Interfaces.Persistence;
using RpgGame.Application.Interfaces.Repositories;
using RpgGame.Application.Interfaces.Services;
using RpgGame.Domain.Events.Users;

namespace RpgGame.Application.Handlers.Events.Users
{
    /// <summary>
    /// Handles UserLoggedInEvent - tracks login analytics and security
    /// </summary>
    public class UserLoggedInEventHandler : IEventHandler<UserLoggedInEvent>
    {
        private readonly ILogger<UserLoggedInEventHandler> _logger;
        private readonly IAnalyticsService _analyticsService;
        private readonly ISecurityService _securityService;

        public UserLoggedInEventHandler(
            ILogger<UserLoggedInEventHandler> logger,
            IAnalyticsService analyticsService,
            ISecurityService securityService)
        {
            _logger = logger;
            _analyticsService = analyticsService;
            _securityService = securityService;
        }

        public async Task HandleAsync(UserLoggedInEvent domainEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("User {Username} logged in at {LoginTime}", 
                domainEvent.Username, domainEvent.LoginTime);

            try
            {
                // Track login analytics
                await _analyticsService.TrackUserLoginAsync(
                    domainEvent.AggregateId,
                    domainEvent.Username,
                    domainEvent.LoginTime,
                    domainEvent.IpAddress,
                    cancellationToken);

                // Security monitoring for suspicious login patterns
                await _securityService.MonitorLoginAsync(
                    domainEvent.AggregateId,
                    domainEvent.IpAddress,
                    domainEvent.UserAgent,
                    domainEvent.LoginTime,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling UserLoggedInEvent for user {Username}", domainEvent.Username);
            }
        }
    }
}