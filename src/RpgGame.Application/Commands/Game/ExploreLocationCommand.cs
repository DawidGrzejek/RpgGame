using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using RpgGame.Application.Commands.Results;
using RpgGame.Application.Events;
using RpgGame.Domain.Common;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Interfaces.World;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RpgGame.Application.Commands.Game
{
    public class ExploreLocationCommand : ICommand<OperationResult<ExploreResult>>
    {
        public Guid CharacterId { get; set; }
        public string LocationName { get; set; } = string.Empty;
    }

    public class ExploreLocationCommandValidator : AbstractValidator<ExploreLocationCommand>
    {
        public ExploreLocationCommandValidator()
        {
            RuleFor(x => x.CharacterId)
                .NotEmpty()
                .WithMessage("Character ID is required");

            RuleFor(x => x.LocationName)
                .NotEmpty()
                .WithMessage("Location name is required");
        }
    }

    public class ExploreLocationCommandHandler : IRequestHandler<ExploreLocationCommand, OperationResult<ExploreResult>>
    {
        private readonly IEventSourcingService _eventSourcingService;
        private readonly IGameWorld _gameWorld;
        private readonly ILogger<ExploreLocationCommandHandler> _logger;

        public ExploreLocationCommandHandler(
            IEventSourcingService eventSourcingService,
            IGameWorld gameWorld,
            ILogger<ExploreLocationCommandHandler> logger)
        {
            _eventSourcingService = eventSourcingService;
            _gameWorld = gameWorld;
            _logger = logger;
        }

        public async Task<OperationResult<ExploreResult>> Handle(ExploreLocationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var character = await _eventSourcingService.GetByIdAsync<Character>(request.CharacterId);
                if (character == null)
                    return OperationResult<ExploreResult>.NotFound("Character", request.CharacterId.ToString());

                var location = _gameWorld.GetLocation(request.LocationName);
                if (location == null)
                    return OperationResult<ExploreResult>.NotFound("Location", request.LocationName);

                var result = new ExploreResult();

                // Random encounter logic
                var random = new Random();
                var encounterRoll = random.NextDouble();

                if (encounterRoll < 0.3 && location.PossibleEnemies.Count > 0) // 30% chance
                {
                    // Enemy encounter
                    var randomEnemy = location.GetRandomEnemy();
                    result.EnemyEncountered = true;
                    result.Enemy = randomEnemy;
                    result.Message = $"You encounter a {randomEnemy?.Name}!";
                }
                else if (encounterRoll < 0.1) // 10% chance for item
                {
                    // Item found
                    result.ItemFound = true;
                    result.ItemName = "Health Potion"; // Simplified - would be random
                    result.Message = $"You found a {result.ItemName}!";
                }
                else if (encounterRoll < 0.2) // 10% chance for experience
                {
                    // Experience gained from exploration
                    result.ExperienceGained = random.Next(5, 15);
                    result.Message = $"You gain {result.ExperienceGained} experience from exploration.";

                    if (character.Type == RpgGame.Domain.Enums.CharacterType.Player)
                    {
                        character.GainExperience(result.ExperienceGained);
                        await _eventSourcingService.SaveAsync(character);
                    }
                }
                else
                {
                    // Nothing found
                    result.Message = "You find nothing of interest.";
                }

                _logger.LogInformation("Character {CharacterName} explored {LocationName}: {Message}",
                    character.Name, request.LocationName, result.Message);

                return OperationResult<ExploreResult>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exploring location {LocationName} with character {CharacterId}",
                    request.LocationName, request.CharacterId);
                return OperationResult<ExploreResult>.Failure("Exploration.Failed", ex.Message);
            }
        }
    }
}