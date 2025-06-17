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
    public class MoveCharacterCommand : ICommand<OperationResult<LocationChangeResult>>
    {
        public Guid CharacterId { get; set; }
        public string TargetLocation { get; set; } = string.Empty;
    }

    public class MoveCharacterCommandValidator : AbstractValidator<MoveCharacterCommand>
    {
        public MoveCharacterCommandValidator()
        {
            RuleFor(x => x.CharacterId)
                .NotEmpty()
                .WithMessage("Character ID is required");

            RuleFor(x => x.TargetLocation)
                .NotEmpty()
                .WithMessage("Target location is required");
        }
    }

    public class MoveCharacterCommandHandler : IRequestHandler<MoveCharacterCommand, OperationResult<LocationChangeResult>>
    {
        private readonly IEventSourcingService _eventSourcingService;
        private readonly IGameWorld _gameWorld;
        private readonly ILogger<MoveCharacterCommandHandler> _logger;

        public MoveCharacterCommandHandler(
            IEventSourcingService eventSourcingService,
            IGameWorld gameWorld,
            ILogger<MoveCharacterCommandHandler> logger)
        {
            _eventSourcingService = eventSourcingService;
            _gameWorld = gameWorld;
            _logger = logger;
        }

        public async Task<OperationResult<LocationChangeResult>> Handle(MoveCharacterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var character = await _eventSourcingService.GetByIdAsync<Character>(request.CharacterId);
                if (character == null)
                    return OperationResult<LocationChangeResult>.NotFound("Character", request.CharacterId.ToString());

                var targetLocation = _gameWorld.GetLocation(request.TargetLocation);
                if (targetLocation == null)
                    return OperationResult<LocationChangeResult>.NotFound("Location", request.TargetLocation);

                // In a real implementation, you'd check if the move is valid
                // (e.g., is the target location connected to current location?)

                var result = new LocationChangeResult
                {
                    NewLocation = targetLocation,
                    Success = true,
                    Message = $"Moved to {targetLocation.Name}"
                };

                _logger.LogInformation("Character {CharacterName} moved to {LocationName}",
                    character.Name, targetLocation.Name);

                return OperationResult<LocationChangeResult>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error moving character {CharacterId} to {TargetLocation}",
                    request.CharacterId, request.TargetLocation);
                return OperationResult<LocationChangeResult>.Failure("Movement.Failed", ex.Message);
            }
        }
    }
}