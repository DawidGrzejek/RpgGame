using FluentValidation;
using MediatR;
using RpgGame.Application.Events;
using RpgGame.Domain.Entities.Characters.Base;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RpgGame.Application.Queries.Characters
{
    public class GetCharacterByIdQuery : IQuery<QueryResult<Character>>
    {
        public Guid CharacterId { get; set; }
    }

    public class GetCharacterByIdQueryValidator : AbstractValidator<GetCharacterByIdQuery>
    {
        public GetCharacterByIdQueryValidator()
        {
            RuleFor(q => q.CharacterId)
                .NotEmpty().WithMessage("Character ID is required");
        }
    }

    public class GetCharacterByIdQueryHandler : IRequestHandler<GetCharacterByIdQuery, QueryResult<Character>>
    {
        private readonly IEventSourcingService _eventSourcingService;

        public GetCharacterByIdQueryHandler(IEventSourcingService eventSourcingService)
        {
            _eventSourcingService = eventSourcingService;
        }

        public async Task<QueryResult<Character>> Handle(GetCharacterByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var character = await _eventSourcingService.GetByIdAsync<Character>(request.CharacterId);

                if (character == null)
                {
                    return QueryResult<Character>.Fail($"Character with ID {request.CharacterId} not found");
                }

                return QueryResult<Character>.Ok(character);
            }
            catch (Exception ex)
            {
                return QueryResult<Character>.Fail($"Error retrieving character: {ex.Message}");
            }
        }
    }
}