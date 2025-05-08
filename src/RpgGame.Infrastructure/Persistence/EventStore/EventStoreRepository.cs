using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RpgGame.Domain.Events.Base;
using RpgGame.Infrastructure.Persistence.EFCore;
using RpgGame.Infrastructure.Persistence.EventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RpgGame.Infrastructure.EventStore
{
    public class EventStoreRepository : IEventStoreRepository
    {
        private readonly GameDbContext _context;
        private readonly JsonSerializerSettings _jsonSettings;

        public EventStoreRepository(GameDbContext context)
        {
            _context = context;
            _jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };
        }

        public async Task<IEnumerable<IDomainEvent>> GetEventsAsync(Guid aggregateId)
        {
            var storedEvents = await _context.Events
                .Where(e => e.AggregateId == aggregateId)
                .OrderBy(e => e.Version)
                .ToListAsync();

            return storedEvents.Select(DeserializeEvent);
        }

        public async Task<IEnumerable<IDomainEvent>> GetEventsAsync(Guid aggregateId, int fromVersion)
        {
            var storedEvents = await _context.Events
                .Where(e => e.AggregateId == aggregateId && e.Version >= fromVersion)
                .OrderBy(e => e.Version)
                .ToListAsync();

            return storedEvents.Select(DeserializeEvent);
        }

        public async Task SaveEventAsync(IDomainEvent @event, string userId = null)
        {
            var storedEvent = CreateStoredEvent(@event, userId);
            await _context.Events.AddAsync(storedEvent);
            await _context.SaveChangesAsync();
        }

        public async Task SaveEventsAsync(IEnumerable<IDomainEvent> events, string userId = null)
        {
            var storedEvents = events.Select(e => CreateStoredEvent(e, userId));
            await _context.Events.AddRangeAsync(storedEvents);
            await _context.SaveChangesAsync();
        }

        private StoredEvent CreateStoredEvent(IDomainEvent @event, string userId)
        {
            return new StoredEvent
            {
                Id = @event.EventId,
                AggregateId = @event.AggregateId,
                AggregateType = @event.GetType().DeclaringType?.Name ?? "Unknown",
                Version = @event.Version,
                EventType = @event.EventType,
                EventData = JsonConvert.SerializeObject(@event, _jsonSettings),
                Timestamp = @event.OccurredAt,
                UserId = userId
            };
        }

        private IDomainEvent DeserializeEvent(StoredEvent storedEvent)
        {
            return JsonConvert.DeserializeObject<IDomainEvent>(storedEvent.EventData, _jsonSettings);
        }
    }
}