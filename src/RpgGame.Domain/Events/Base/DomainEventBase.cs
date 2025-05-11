using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Domain.Events.Base
{
    /// <summary>
    /// Base class for domain events
    /// </summary>
    public abstract class DomainEventBase : IDomainEvent
    {
        public Guid EventId { get; }
        public DateTime OccurredAt { get; }
        public string EventType { get; }
        public Guid AggregateId { get; }
        public int Version { get; }

        protected DomainEventBase(Guid aggregateId, int version = 1)
        {
            EventId = Guid.NewGuid();
            OccurredAt = DateTime.UtcNow;
            EventType = GetType().Name;
            AggregateId = aggregateId;
            Version = version;
        }
    }
}
