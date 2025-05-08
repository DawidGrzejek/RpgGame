using FluentValidation;
using MediatR;
using RpgGame.Application.Events;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Interfaces.Items;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RpgGame.Application.Queries.Inventory
{
    public class GetCharacterInventoryQuery : IQuery<QueryResult<IReadOnlyList<IItem>>>
    {
        public Guid CharacterId { get; set; }
    }

    public class GetCharacterInventoryQueryValidator : AbstractValidator<GetCharacterInventoryQuery>
    {
        public GetCharacterInventoryQueryValidator()
        {
            RuleFor(q => q.CharacterId)
                .NotEmpty().WithMessage("Character ID is required");
        }
    }

    public class GetCharacterInventoryQueryHandler : IRequestHandler<GetCharacterInventoryQuery, QueryResult<IReadOnlyList<IItem>>>
    {
        private readonly IEventSourcingService _eventSourcingService;

        public GetCharacterInventoryQueryHandler(IEventSourcingService eventSourcingService)
        {
            _eventSourcingService = eventSourcingService;
        }

        public async Task<QueryResult<IReadOnlyList<IItem>>> Handle(GetCharacterInventoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var character = await _eventSourcingService.GetByIdAsync<PlayerCharacter>(request.CharacterId);

                if (character == null)
                {
                    return QueryResult<IReadOnlyList<IItem>>.Fail($"Character with ID {request.CharacterId} not found");
                }

                return QueryResult<IReadOnlyList<IItem>>.Ok(character.Inventory.Items);
            }
            catch (Exception ex)
            {
                return QueryResult<IReadOnlyList<IItem>>.Fail($"Error retrieving inventory: {ex.Message}");
            }
        }
    }
}