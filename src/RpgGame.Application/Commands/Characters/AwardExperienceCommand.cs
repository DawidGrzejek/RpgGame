using MediatR;
using Microsoft.Extensions.Logging;
using RpgGame.Application.Events;
using RpgGame.Domain.Common;
using RpgGame.Domain.Entities.Characters.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Application.Commands.Characters
{
    public class AwardExperienceCommand : ICommand<OperationResult>
    {
        public Guid CharacterId { get; set; }
        public int ExperienceAmount { get; set; }
        public string Source { get; set; } = string.Empty;
    }

    public class AwardExperienceCommandHandler : IRequestHandler<AwardExperienceCommand, OperationResult>
    {
        private readonly IEventSourcingService _eventSourcingService;
        private readonly ILogger<AwardExperienceCommandHandler> _logger;

        public AwardExperienceCommandHandler(
            IEventSourcingService eventSourcingService,
            ILogger<AwardExperienceCommandHandler> logger)
        {
            _eventSourcingService = eventSourcingService;
            _logger = logger;
        }

        public async Task<OperationResult> Handle(AwardExperienceCommand request, CancellationToken cancellationToken)
        {
            var character = await _eventSourcingService.GetByIdAsync<Character>(request.CharacterId);
            if (character == null)
            {
                return OperationResult.NotFound("Character", request.CharacterId.ToString());
            }

            // Only players can gain experience
            if (character.Type != RpgGame.Domain.Enums.CharacterType.Player)
            {
                return OperationResult.Failure("Character.NotPlayer", "Character is not a player and cannot gain experience");
            }

            character.GainExperience(request.ExperienceAmount);

            await _eventSourcingService.SaveAsync(character);

            _logger.LogInformation("Awarded {Experience} experience to {Character} from {Source}",
                request.ExperienceAmount, character.Name, request.Source);

            return OperationResult.Success();
        }
    }
}
