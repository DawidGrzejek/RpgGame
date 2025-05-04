using RpgGame.Domain.Events.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Application.Events
{
    public class InMemoryEventPublisher : IEventPublisher
    {
        public readonly Dictionary<Type, List<IEventHandlerWrapper>> _handlers = new();
        public readonly object _lock = new();

        /// <summary>
        /// Publishes a domain event to all registered handlers.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="domainEvent"></param>
        /// <returns></returns>
        public Task PublishAsync<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent
        {
            List<IEventHandlerWrapper> eventHandlers = null;

            lock (_lock)
            {
                if (_handlers.TryGetValue(typeof(TEvent), out var handlers))
                {
                    eventHandlers = handlers.ToList();
                }
            }

            if (eventHandlers != null)
            {
                var tasks = eventHandlers
                    .Cast<IEventHandlerWrapper<TEvent>>()
                    .Select(handler => handler.HandleAsync(domainEvent));

                return Task.WhenAll(tasks);
            }

            // Console logging for debugging purposes
            Console.WriteLine($"Event Published: {domainEvent.EventType} at {domainEvent.OccurredAt}");
        }

        /// <summary>
        /// Registers a handler for a specific domain event type.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="handler"></param>
        public void RegisterHandler<TEvent>(IEventHandler<TEvent> handler) where TEvent : IDomainEvent
        {
            lock (_lock)
            {
                if (!_handlers.TryGetValue(typeof(TEvent), out var handlers))
                {
                    handlers = new List<IEventHandlerWrapper>();
                    _handlers[typeof(TEvent)] = handlers;
                }

                handlers.Add(new EventHandlerWrapper<TEvent>(handler));
            }
        }

        /// <summary>
        /// Removes a handler for a specific domain event type.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="handler"></param>
        public void RemoveHandler<TEvent>(IEventHandler<TEvent> handler) where TEvent : IDomainEvent
        {
            lock (_lock)
            {
                if (_handlers.TryGetValue(typeof(TEvent), out var handlers))
                {
                    handlers.RemoveAll(h => h is EventHandlerWrapper<TEvent> wrapper
                                       && wrapper.Handler == handler);

                    if (handlers.Count == 0)
                    {
                        _handlers.Remove(typeof(TEvent));
                    }
                }
            }
        }

        private interface IEventHandlerWrapper { }

        private interface IEventHandlerWrapper<TEvent> : IEventHandlerWrapper where TEvent : IDomainEvent
        {
            Task HandleAsync(TEvent domainEvent);
        }

        private class EventHandlerWrapper<TEvent> : IEventHandlerWrapper<TEvent> where TEvent : IDomainEvent
        {
            public IEventHandler<TEvent> Handler { get; }

            public EventHandlerWrapper(IEventHandler<TEvent> handler)
            {
                Handler = handler;
            }

            public async Task HandleAsync(TEvent domainEvent)
            {
                await Handler.HandleAsync(domainEvent);
            }
        }
    }
}
