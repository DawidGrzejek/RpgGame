using Microsoft.Extensions.Logging;
using RpgGame.Application.Interfaces.Services;
using RpgGame.Domain.Events.Combat;
using RpgGame.Domain.Interfaces.Quests;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RpgGame.Application.Events.Handlers.Combat
{
    public class CombatVictoryQuestProgressHandler : IEventHandler<CombatVictoryEvent>
    {
        private readonly IQuestService _questService;
        private readonly ILogger<CombatVictoryQuestProgressHandler> _logger;

        public CombatVictoryQuestProgressHandler(
            IQuestService questService,
            ILogger<CombatVictoryQuestProgressHandler> logger)
        {
            _questService = questService;
            _logger = logger;
        }

        public async Task HandleAsync(CombatVictoryEvent domainEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                // Update any active quests that require defeating this enemy type
                //await _questService.UpdateCombatObjectivesAsync(
                //    domainEvent.PlayerName,
                //    domainEvent.EnemyName);

                _logger.LogInformation(
                    "Updated quest progress for player {PlayerName} after defeating {EnemyName}",
                    domainEvent.PlayerName,
                    domainEvent.EnemyName);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error updating quest progress for player {PlayerName} after defeating {EnemyName}",
                    domainEvent.PlayerName,
                    domainEvent.EnemyName);
            }
        }
    }
}