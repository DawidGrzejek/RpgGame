using MediatR;
using RpgGame.Application.Commands;
using RpgGame.Application.Events;
using RpgGame.Domain.Base;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ICommand = RpgGame.Application.Commands.ICommand;

namespace RpgGame.Application.Behaviors
{
    public class EventSourcingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEventDispatcher _eventDispatcher;

        public EventSourcingBehavior(IEventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Process the command/query
            var response = await next();

            // If this is a command that returns an entity with domain events, dispatch them
            if (request is ICommand && response is DomainEntity entity)
            {
                var events = entity.DomainEvents.ToList();
                if (events.Any())
                {
                    await _eventDispatcher.DispatchAsync(events, cancellationToken);
                    entity.ClearDomainEvents();
                }
            }

            return response;
        }
    }
}