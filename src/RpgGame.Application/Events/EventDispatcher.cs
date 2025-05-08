using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RpgGame.Application.Events;
using RpgGame.Domain.Events.Base;

public class EventDispatcher : IEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventDispatcher> _logger;
    private readonly IEventStoreRepository _eventStore;

    public EventDispatcher(
        IServiceProvider serviceProvider,
        ILogger<EventDispatcher> logger,
        IEventStoreRepository eventStore)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _eventStore = eventStore;
    }

    public async Task DispatchAsync(IDomainEvent @event, CancellationToken cancellationToken = default)
    {
        // First, store the event
        await _eventStore.SaveEventAsync(@event);

        var eventType = @event.GetType();
        _logger.LogInformation("Dispatching domain event {EventType}", eventType.Name);

        // Then, dispatch to handlers
        var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
        var handlers = _serviceProvider.GetServices(handlerType);

        if (!handlers.Any())
        {
            _logger.LogWarning("No handlers registered for {EventType}", eventType.Name);
            return;
        }

        foreach (var handler in handlers)
        {
            try
            {
                await (Task)handlerType
                    .GetMethod("HandleAsync")?
                    .Invoke(handler, new object[] { @event, cancellationToken });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling {EventType}", eventType.Name);
            }
        }
    }

    public async Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default)
    {
        foreach (var @event in events)
        {
            await DispatchAsync(@event, cancellationToken);
        }
    }
}