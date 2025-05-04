using RpgGame.Domain.Events.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Application.Events
{
    /// <summary>
    /// Publishes domain events to registered subscribers.
    /// </summary>
    public interface IEventPublisher
    {
        Task PublishAsync<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent;
        void RegisterHandler<TEvent>(IEventHandler<TEvent> handler) where TEvent : IDomainEvent;
        void RemoveHanlder<TEvent>(IEventHandler<TEvent> handler) where TEvent : IDomainEvent;
    }
}
