using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RpgGame.Domain.Events.Characters;

namespace RpgGame.Application.Events.Handlers.Character
{
    public class CharacterLeveledUpHandler : IEventHandler<CharacterLeveledUp>
    {
        private readonly ILogger<CharacterLeveledUpHandler> _logger;
        // Inject other dependencies like repositories if needed

        public CharacterLeveledUpHandler(ILogger<CharacterLeveledUpHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(CharacterLeveledUp @event, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Character {CharacterName} leveled up from {@OldLevel} to {@NewLevel}",
                @event.CharacterName,
                @event.OldLevel,
                @event.NewLevel);

            // Apply level-up bonuses, check for unlocked abilities, etc.

            // You could trigger achievements
            if (@event.NewLevel >= 10)
            {
                _logger.LogInformation("Character {CharacterName} earned 'Reaching for the Stars' achievement",
                    @event.CharacterName);
                // Could trigger another event for achievement unlocked
            }

            await Task.CompletedTask; // Replace with actual async work
        }
    }
}
