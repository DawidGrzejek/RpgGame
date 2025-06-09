using RpgGame.Domain.Events.Base;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RpgGame.Application.Events
{
    /// <summary>
    /// Repository interface for event storage, defined in the Application layer
    /// but implemented in the Infrastructure layer
    /// </summary>
    public interface IEventStoreRepository
    {
        /// <summary>
        /// Gets all events for a specific aggregate
        /// </summary>
        Task<IEnumerable<IDomainEvent>> GetEventsAsync(Guid aggregateId);

        /// <summary>
        /// Gets events for a specific aggregate starting from a specific version
        /// </summary>
        Task<IEnumerable<IDomainEvent>> GetEventsAsync(Guid aggregateId, int fromVersion);

        /// <summary>
        /// Saves a single event to the event store
        /// </summary>
        Task SaveEventAsync(IDomainEvent @event, string userId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves multiple events to the event store
        /// </summary>
        Task SaveEventsAsync(IEnumerable<IDomainEvent> events, string userId = null, CancellationToken cancellationToken = default);
    }
}