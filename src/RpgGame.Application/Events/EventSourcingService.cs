using RpgGame.Domain.Base;
using RpgGame.Domain.Events.Base;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RpgGame.Application.Events
{
    public class EventSourcingService : IEventSourcingService
    {
        private readonly IEventStoreRepository _eventStore;
        private readonly IEventDispatcher _eventDispatcher;

        public EventSourcingService(
            IEventStoreRepository eventStore,
            IEventDispatcher eventDispatcher)
        {
            _eventStore = eventStore;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<T> GetByIdAsync<T>(Guid id) where T : DomainEntity, new()
        {
            var events = await _eventStore.GetEventsAsync(id);
            if (!events.Any())
                return null;

            var aggregate = new T();

            // Set the Id property using reflection (since it's protected)
            typeof(T).GetProperty("Id")?.SetValue(aggregate, id);

            // Apply all events to rebuild the state
            foreach (var @event in events)
            {
                ApplyEventToAggregate(aggregate, @event);
            }

            return aggregate;
        }

        public async Task SaveAsync<T>(T aggregate) where T : DomainEntity
        {
            var events = aggregate.DomainEvents.ToList();
            if (!events.Any())
                return;

            // Save all new events to the event store
            await _eventStore.SaveEventsAsync(events);

            // Dispatch events to handlers
            await _eventDispatcher.DispatchAsync(events);

            // Clear events from the aggregate
            aggregate.ClearDomainEvents();
        }

        private void ApplyEventToAggregate<T>(T aggregate, IDomainEvent @event) where T : DomainEntity
        {
            // Find the Apply method for this event type
            var eventType = @event.GetType();
            var applyMethod = typeof(T).GetMethod("Apply", new[] { eventType });

            if (applyMethod != null)
            {
                applyMethod.Invoke(aggregate, new object[] { @event });
            }
            else
            {
                throw new InvalidOperationException(
                    $"Aggregate {typeof(T).Name} does not have an Apply method for event {eventType.Name}");
            }
        }
    }
}