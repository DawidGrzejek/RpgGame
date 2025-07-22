using FluentValidation;
using MediatR;
using RpgGame.Application.Events;
using RpgGame.Application.Interfaces.Repositories;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Interfaces.Items;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RpgGame.Application.Commands.Inventory
{
    public class UseItemCommand : ICommand<CommandResult>
    {
        public Guid CharacterId { get; set; }
        public Guid ItemId { get; set; }
    }

    public class UseItemCommandValidator : AbstractValidator<UseItemCommand>
    {
        public UseItemCommandValidator()
        {
            RuleFor(c => c.CharacterId)
                .NotEmpty().WithMessage("Character ID is required");

            RuleFor(c => c.ItemId)
                .NotEmpty().WithMessage("Item ID is required");
        }
    }

    public class UseItemCommandHandler : IRequestHandler<UseItemCommand, CommandResult>
    {
        private readonly IEventSourcingService _eventSourcingService;
        private readonly IItemRepository _itemRepository;

        public UseItemCommandHandler(
            IEventSourcingService eventSourcingService,
            IItemRepository itemRepository)
        {
            _eventSourcingService = eventSourcingService;
            _itemRepository = itemRepository;
        }

        public async Task<CommandResult> Handle(UseItemCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get character from event store
                var character = await _eventSourcingService.GetByIdAsync<Character>(request.CharacterId);

                if (character == null)
                {
                    return CommandResult.Fail($"Character with ID {request.CharacterId} not found");
                }

                // Check if it's a PlayerCharacter since only players can use items
                if (character is not PlayerCharacter playerCharacter)
                {
                    return CommandResult.Fail($"Character with ID {request.CharacterId} is not a player character");
                }

                // Get item from repository
                var item = await _itemRepository.GetByIdAsync(request.ItemId);

                if (item == null)
                {
                    return CommandResult.Fail($"Item with ID {request.ItemId} not found");
                }

                // Check if item is consumable
                if (item is not IConsumable)
                {
                    return CommandResult.Fail($"Item '{item.Name}' is not consumable");
                }

                // Use the item - this will generate events in the character's DomainEvents collection
                playerCharacter.UseItem(item);

                // Save the updated character with its new events
                await _eventSourcingService.SaveAsync(playerCharacter);

                return CommandResult.Ok($"Character {playerCharacter.Name} used {item.Name}");
            }
            catch (Exception ex)
            {
                return CommandResult.Fail($"Failed to use item: {ex.Message}");
            }
        }
    }
}