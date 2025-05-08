using FluentValidation;
using MediatR;
using RpgGame.Domain.Events.Base;
using RpgGame.Infrastructure.EventStore;
using RpgGame.Infrastructure.Persistence.EventStore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RpgGame.Application.Queries.Characters
{
    public class GetCharacterHistoryQuery : IQuery<QueryResult<IEnumerable<IDomainEvent>>>
    {
        public Guid CharacterId { get; set; }
    }

    public class GetCharacterHistoryQueryValidator : AbstractValidator<GetCharacterHistoryQuery>
    {
        public GetCharacterHistoryQueryValidator()
        {
            RuleFor(q => q.CharacterId)
                .NotEmpty().WithMessage("Character ID is required");
        }
    }

    public class GetCharacterHistoryQueryHandler : IRequestHandler<GetCharacterHistoryQuery, QueryResult<IEnumerable<IDomainEvent>>>
    {
        private readonly IEventStoreRepository _eventStoreRepository;

        public GetCharacterHistoryQueryHandler(IEventStoreRepository eventStoreRepository)
        {
            _eventStoreRepository = eventStoreRepository;
        }

        public async Task<QueryResult<IEnumerable<IDomainEvent>>> Handle(GetCharacterHistoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var events = await _eventStoreRepository.GetEventsAsync(request.CharacterId);
                return QueryResult<IEnumerable<IDomainEvent>>.Ok(events);
            }
            catch (Exception ex)
            {
                return QueryResult<IEnumerable<IDomainEvent>>.Fail($"Error retrieving character history: {ex.Message}");
            }
        }
    }
}