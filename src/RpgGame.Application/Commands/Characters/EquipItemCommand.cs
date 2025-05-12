using FluentValidation;
using MediatR;
using RpgGame.Application.Events;
using RpgGame.Application.Repositories;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Interfaces.Items;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RpgGame.Application.Commands.Inventory
{
    public class EquipItemCommand : ICommand<CommandResult>
    {
        public Guid CharacterId { get; set; }
        public Guid ItemId { get; set; }
    }

    public class EquipItemCommandValidator : AbstractValidator<EquipItemCommand>
    {
        public EquipItemCommandValidator()
        {
            RuleFor(c => c.CharacterId)
                .NotEmpty().WithMessage("Character ID is required");

            RuleFor(c => c.ItemId)
                .NotEmpty().WithMessage("Item ID is required");
        }
    }

    public class EquipItemCommandHandler : IRequestHandler<EquipItemCommand, CommandResult>
    {
        private readonly IEventSourcingService _eventSourcingService;
        private readonly IItemRepository _itemRepository;

        public EquipItemCommandHandler(
            IEventSourcingService eventSourcingService,
            IItemRepository itemRepository)
        {
            _eventSourcingService = eventSourcingService;
            _itemRepository = itemRepository;
        }

        public async Task<CommandResult> Handle(EquipItemCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get character - this will return the concrete character type from the event stream
                var character = await _eventSourcingService.GetByIdAsync<Character>(request.CharacterId);

                if (character == null)
                {
                    return CommandResult.Fail($"Character with ID {request.CharacterId} not found");
                }

                // Check if it's a PlayerCharacter since only players can equip items
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

                // Check if item is equipment
                if (item is not IEquipment equipment)
                {
                    return CommandResult.Fail($"Item '{item.Name}' is not equipable");
                }

                // Equip the item - this will generate an event in the character's DomainEvents collection
                playerCharacter.EquipItem(equipment);

                // Save the updated character with its new event
                await _eventSourcingService.SaveAsync(playerCharacter);

                return CommandResult.Ok($"Character {playerCharacter.Name} equipped {item.Name}");
            }
            catch (Exception ex)
            {
                return CommandResult.Fail($"Failed to equip item: {ex.Message}");
            }
        }
    }
}