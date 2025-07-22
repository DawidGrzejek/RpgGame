using Microsoft.Extensions.Logging;
using RpgGame.Application.Events;
using RpgGame.Application.Interfaces.Persistence;
using RpgGame.Application.Interfaces.Repositories;
using RpgGame.Application.Interfaces.Services;
using RpgGame.Domain.Events.Users;

namespace RpgGame.Application.Handlers.Events.Users
{/// <summary>
    /// Handles CharacterAssignedToUserEvent - updates user statistics and sends notifications
    /// </summary>
    public class CharacterAssignedToUserEventHandler : IEventHandler<CharacterAssignedToUserEvent>
    {
        private readonly ILogger<CharacterAssignedToUserEventHandler> _logger;
        private readonly IUserRepository _userRepository;
        // private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;

        public CharacterAssignedToUserEventHandler(
            ILogger<CharacterAssignedToUserEventHandler> logger,
            IUserRepository userRepository,
            // INotificationService notificationService,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _userRepository = userRepository;
            // _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(CharacterAssignedToUserEvent domainEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Character {CharacterName} ({CharacterType}) assigned to user {Username}", 
                domainEvent.CharacterName, domainEvent.CharacterType, domainEvent.Username);

            try
            {
                // Update user statistics
                var user = await _userRepository.GetByIdAsync(domainEvent.AggregateId);
                if (user != null)
                {
                    // Update statistics to reflect character creation
                    var updatedStats = user.Statistics.RecordCharacterCreation();
                    user.UpdateStatistics(updatedStats);
                    
                    await _userRepository.UpdateAsync(user);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    // Check for achievements
                    if (updatedStats.HasCreatedFirstCharacter && updatedStats.CharactersCreated == 1)
                    {
                        // await _notificationService.SendNotificationAsync(
                        //     domainEvent.AggregateId,
                        //     "Achievement Unlocked!",
                        //     "🎉 First Character Created! Your adventure begins now!",
                        //     cancellationToken);
                    }
                }

                // Send character creation notification
                // await _notificationService.SendNotificationAsync(
                //     domainEvent.AggregateId,
                //     "Character Created",
                //     $"Welcome {domainEvent.CharacterName}, the {domainEvent.CharacterType}! Your adventure awaits.",
                //     cancellationToken);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling CharacterAssignedToUserEvent for user {Username}", domainEvent.Username);
            }
        }
    }
}