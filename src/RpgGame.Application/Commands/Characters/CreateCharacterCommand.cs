using FluentValidation;
using MediatR;
using RpgGame.Application.Events;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Entities.Characters.Player;
using RpgGame.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RpgGame.Application.Commands.Characters
{
    public class CreateCharacterCommand : ICommand<CommandResult<Character>>
    {
        public string Name { get; set; }
        public CharacterType Type { get; set; }
    }

    public class CreateCharacterCommandValidator : AbstractValidator<CreateCharacterCommand>
    {
        public CreateCharacterCommandValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Character name is required")
                .MinimumLength(3).WithMessage("Character name must be at least 3 characters long")
                .MaximumLength(20).WithMessage("Character name cannot exceed 20 characters");

            RuleFor(c => c.Type)
                .IsInEnum().WithMessage("Invalid character type");
        }
    }

    public class CreateCharacterCommandHandler : IRequestHandler<CreateCharacterCommand, CommandResult<Character>>
    {
        private readonly IEventDispatcher _eventDispatcher;

        public CreateCharacterCommandHandler(IEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }

        public async Task<CommandResult<Character>> Handle(CreateCharacterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Character character = request.Type switch
                {
                    CharacterType.Warrior => Warrior.Create(request.Name),
                    CharacterType.Mage => Mage.Create(request.Name),
                    CharacterType.Rogue => Rogue.Create(request.Name),
                    _ => throw new ArgumentException("Unsupported character type")
                };

                // Dispatch the character creation event
                await _eventDispatcher.DispatchAsync(character.DomainEvents, cancellationToken);
                character.ClearDomainEvents();

                return CommandResult<Character>.Ok(character, $"{request.Type} character '{request.Name}' created successfully");
            }
            catch (Exception ex)
            {
                return CommandResult<Character>.Fail($"Failed to create character: {ex.Message}");
            }
        }
    }
}