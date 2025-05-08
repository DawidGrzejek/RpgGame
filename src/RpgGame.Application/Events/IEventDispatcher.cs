using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Domain.Events.Base;

namespace RpgGame.Application.Events
{
    public interface IEventDispatcher
    {
        Task DispatchAsync(IDomainEvent @event, CancellationToken cancellationToken = default);
        Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default);
    }
}
