using Microsoft.Extensions.Logging;
using RpgGame.Application.Events;
using RpgGame.Application.Interfaces.Persistence;
using RpgGame.Application.Interfaces.Repositories;
using RpgGame.Application.Interfaces.Services;
using RpgGame.Domain.Events.Users;
namespace RpgGame.Application.Handlers.Events.Users
{

    /// <summary>
    /// Handles UserAchievementUnlockedEvent - celebrates user achievements
    /// </summary>
    public class UserAchievementUnlockedEventHandler : IEventHandler<UserAchievementUnlockedEvent>
    {
        private readonly ILogger<UserAchievementUnlockedEventHandler> _logger;
        // private readonly INotificationService _notificationService;
        private readonly IAchievementService _achievementService;

        public UserAchievementUnlockedEventHandler(
            ILogger<UserAchievementUnlockedEventHandler> logger,
            // INotificationService notificationService,
            IAchievementService achievementService)
        {
            _logger = logger;
            // _notificationService = notificationService;
            _achievementService = achievementService;
        }

        public async Task HandleAsync(UserAchievementUnlockedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("User {Username} unlocked achievement: {AchievementName}", 
                domainEvent.Username, domainEvent.AchievementName);

            try
            {
                // Send achievement notification with celebration
                // await _notificationService.SendNotificationAsync(
                //     domainEvent.AggregateId,
                //     "🏆 Achievement Unlocked!",
                //     $"Congratulations! You've unlocked: {domainEvent.AchievementName}\n{domainEvent.AchievementDescription}",
                //     cancellationToken);

                // Check if this unlocks any additional rewards or achievements
                await _achievementService.CheckForBonusAchievementsAsync(
                    domainEvent.AggregateId, 
                    domainEvent.AchievementName,
                    cancellationToken);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling UserAchievementUnlockedEvent for user {Username}", domainEvent.Username);
            }
        }
    }
}