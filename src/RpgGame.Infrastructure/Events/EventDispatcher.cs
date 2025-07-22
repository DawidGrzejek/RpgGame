using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RpgGame.Application.Events;
using RpgGame.Domain.Events.Base;

namespace RpgGame.Infrastructure.Events
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EventDispatcher> _logger;

        public EventDispatcher(IServiceProvider serviceProvider, ILogger<EventDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task DispatchAsync(IDomainEvent @event, CancellationToken cancellationToken = default)
        {
            await DispatchAsync(new[] { @event }, cancellationToken);
        }

        public async Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default)
        {
            foreach (var @event in events)
            {
                _logger.LogInformation("Dispatching event: {EventType} for aggregate {AggregateId}", 
                    @event.EventType, @event.AggregateId);
                    
                try
                {
                    // Get the event type
                    var eventType = @event.GetType();
                    
                    // Build the handler interface type
                    var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
                    
                    // Get all handlers for this event type
                    var handlers = _serviceProvider.GetServices(handlerType);
                    
                    // Execute all handlers
                    foreach (var handler in handlers)
                    {
                        var handleMethod = handlerType.GetMethod("HandleAsync");
                        if (handleMethod != null)
                        {
                            var task = (Task)handleMethod.Invoke(handler, new object[] { @event, cancellationToken })!;
                            await task;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error dispatching event {EventType} for aggregate {AggregateId}", 
                        @event.EventType, @event.AggregateId);
                    // Don't rethrow - we don't want event handling failures to break the main flow
                }
            }
        }
    }
}