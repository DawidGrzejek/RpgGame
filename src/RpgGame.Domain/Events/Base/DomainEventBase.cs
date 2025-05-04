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

        protected DomainEventBase()
        {
            EventId = Guid.NewGuid();
            OccurredAt = DateTime.UtcNow;
            EventType = GetType().Name;
        }
    }
}
