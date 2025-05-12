using RpgGame.Domain.Base;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Events.Base;
using System;
using System.Linq;
using System.Reflection;
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

        public async Task<T> GetByIdAsync<T>(Guid id) where T : class
        {
            var events = await _eventStore.GetEventsAsync(id);

            if (!events.Any())
                return null;

            if (typeof(Character).IsAssignableFrom(typeof(T)))
            {
                // Use the factory method which knows how to create the right concrete type
                var character = Character.FromEvents(id, events);

                // Cast to the requested type
                return character as T;
            }

            if (!typeof(T).IsAbstract && typeof(DomainEntity).IsAssignableFrom(typeof(T)))
            {
                // Create an instance of the aggregate
                var aggregate = Activator.CreateInstance<T>() as DomainEntity;

                // Apply all events to the aggregate
                foreach (var @event in events)
                {
                    ApplyEventToAggregate(aggregate, @event);
                }

                return aggregate as T;
            }

            throw new NotSupportedException($"Unsupported aggregate type for event sourcing: {typeof(T).Name}");
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
            var applyMethod = aggregate.GetType().GetMethod("Apply",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null, new[] { eventType }, null);

            if (applyMethod != null)
            {
                applyMethod.Invoke(aggregate, new object[] { @event });
            }
            else
            {
                // Log warning if appropriate Apply method isn't found
                // This could be normal if the aggregate doesn't need to react to all event types
            }
        }
    }
}