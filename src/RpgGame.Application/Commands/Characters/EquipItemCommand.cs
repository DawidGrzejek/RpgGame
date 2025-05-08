using FluentValidation;
using MediatR;
using RpgGame.Application.Events;
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
                // Get character from event store
                var character = await _eventSourcingService.GetByIdAsync<PlayerCharacter>(request.CharacterId);

                if (character == null)
                {
                    return CommandResult.Fail($"Character with ID {request.CharacterId} not found");
                }

                // Get item
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

                // Equip the item
                character.EquipItem(equipment);

                // Save the updated character with its new event
                await _eventSourcingService.SaveAsync(character);

                return CommandResult.Ok($"Character {character.Name} equipped {item.Name}");
            }
            catch (Exception ex)
            {
                return CommandResult.Fail($"Failed to equip item: {ex.Message}");
            }
        }
    }
}