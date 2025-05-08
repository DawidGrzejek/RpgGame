using FluentValidation;
using MediatR;
using RpgGame.Application.Events;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Infrastructure.Persistence.EventStore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RpgGame.Application.Commands.Characters
{
    public class LevelUpCharacterCommand : ICommand<CommandResult>
    {
        public Guid CharacterId { get; set; }
    }

    public class LevelUpCharacterCommandValidator : AbstractValidator<LevelUpCharacterCommand>
    {
        public LevelUpCharacterCommandValidator()
        {
            RuleFor(c => c.CharacterId)
                .NotEmpty().WithMessage("Character ID is required");
        }
    }

    public class LevelUpCharacterCommandHandler : IRequestHandler<LevelUpCharacterCommand, CommandResult>
    {
        private readonly IEventStoreRepository _eventStore;
        private readonly IEventDispatcher _eventDispatcher;

        public LevelUpCharacterCommandHandler(
            IEventStoreRepository eventStore,
            IEventDispatcher eventDispatcher)
        {
            _eventStore = eventStore;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<CommandResult> Handle(LevelUpCharacterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get events for this character
                var events = await _eventStore.GetEventsAsync(request.CharacterId);

                if (!events.Any())
                {
                    return CommandResult.Fail($"Character with ID {request.CharacterId} not found");
                }

                // Use factory method to rebuild the character
                var character = Character.FromEvents(request.CharacterId, events);

                if (character == null)
                {
                    return CommandResult.Fail($"Character with ID {request.CharacterId} could not be reconstructed");
                }

                // Level up the character
                character.LevelUp();

                // Dispatch the new event
                await _eventDispatcher.DispatchAsync(character.DomainEvents, cancellationToken);
                character.ClearDomainEvents();

                return CommandResult.Ok($"Character {character.Name} leveled up to level {character.Level}");
            }
            catch (Exception ex)
            {
                return CommandResult.Fail($"Failed to level up character: {ex.Message}");
            }
        }
    }
}