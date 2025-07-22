using MediatR;
using Microsoft.Extensions.Logging;
using RpgGame.Application.Commands.Characters;
using RpgGame.Application.Events;
using RpgGame.Application.Services;
using RpgGame.Domain.Common;
using RpgGame.Domain.Events.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Application.Handlers.Events.Character
{
    /// <summary>
     /// Handles player death events
     /// </summary>
    public class CharacterDiedHandler : IEventHandler<CharacterDied>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CharacterDiedHandler> _logger;

        public CharacterDiedHandler(
            IMediator mediator,
            ILogger<CharacterDiedHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task HandleAsync(CharacterDied @event, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Character {CharacterName} died at location {Location}",
                @event.CharacterName, @event.Location);

            // The domain event triggers the complex death processing command
            var deathCommand = new ProcessCharacterDeathCommand
            {
                CharacterId = @event.AggregateId,
                LocationName = @event.Location,
                CauseOfDeath = "Unknown", // Could be enhanced to pass this info
                IsPlayerCharacter = true // Could be determined from character type
            };

            var result = await _mediator.Send<OperationResult<CharacterDeathResult>>(deathCommand, cancellationToken);

            if (!result.Succeeded)
            {
                _logger.LogError("Failed to process character death: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}
