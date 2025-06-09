using RpgGame.Application.Events;
using RpgGame.Domain.Events.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RpgGame.IntegrationTests
{
    /// <summary>
    /// A simple in-memory event store repository for testing
    /// </summary>
    public class TestEventStoreRepository : IEventStoreRepository
    {
        private readonly List<StoredEvent> _events = new List<StoredEvent>();

        public Task<IEnumerable<IDomainEvent>> GetEventsAsync(Guid aggregateId)
        {
            return Task.FromResult<IEnumerable<IDomainEvent>>(new List<IDomainEvent>());
        }

        public Task<IEnumerable<IDomainEvent>> GetEventsAsync(Guid aggregateId, int fromVersion)
        {
            return Task.FromResult<IEnumerable<IDomainEvent>>(new List<IDomainEvent>());
        }

        public Task SaveEventAsync(IDomainEvent @event, string userId = null, CancellationToken cancellationToken = default)
        {
            var storedEvent = new StoredEvent
            {
                Id = @event.EventId,
                AggregateId = @event.AggregateId,
                AggregateType = @event.GetType().DeclaringType?.Name ?? "Unknown",
                Version = @event.Version,
                EventType = @event.EventType,
                EventData = "Test Event Data", // Simplified for test
                Timestamp = @event.OccurredAt,
                UserId = userId
            };

            _events.Add(storedEvent);
            return Task.CompletedTask;
        }

        public Task SaveEventsAsync(IEnumerable<IDomainEvent> events, string userId = null, CancellationToken cancellationToken = default)
        {
            foreach (var @event in events)
            {
                SaveEventAsync(@event, userId, cancellationToken).Wait();
            }
            return Task.CompletedTask;
        }
    }
}