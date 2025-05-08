using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Domain.Events.Base;

namespace RpgGame.Application.Events
{
    public interface IEventStoreRepository
    {
        Task<IEnumerable<IDomainEvent>> GetEventsAsync(Guid aggregateId);
        Task<IEnumerable<IDomainEvent>> GetEventsAsync(Guid aggregateId, int fromVersion);
        Task SaveEventAsync(IDomainEvent @event, string userId = null);
        Task SaveEventsAsync(IEnumerable<IDomainEvent> events, string userId = null);
    }
}
