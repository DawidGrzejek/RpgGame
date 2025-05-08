using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Domain.Base;

namespace RpgGame.Application.Events
{
    public interface IEventSourcingService
    {
        Task<T> GetByIdAsync<T>(Guid id) where T : DomainEntity, new();
        Task SaveAsync<T>(T aggregate) where T : DomainEntity;
    }
}
