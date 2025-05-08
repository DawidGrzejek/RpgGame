using RpgGame.Domain.Events.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Domain.Base
{
    /// <summary>
    /// Base class for all domain entities that can raise events
    /// </summary>
    public abstract class DomainEntity : IHasDomainEvents
    {
        private readonly List<IDomainEvent> _domainEvents = new();
        private int _version = 0;

        // Identifier for event tracking
        public Guid Id { get; protected set; } = Guid.Empty;

        // Track entitiy version
        public int Version => _version;

        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
            _version++;
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        protected TEvent RaiseDomainEvent<TEvent>(Func<Guid, int, TEvent> factory)
            where TEvent : IDomainEvent
        {
            var @event = factory(Id, _version + 1);
            AddDomainEvent(@event);
            return @event;
        }
    }
}
