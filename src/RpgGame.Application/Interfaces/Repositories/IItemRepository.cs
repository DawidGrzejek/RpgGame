using RpgGame.Domain.Interfaces.Items;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RpgGame.Application.Interfaces.Repositories
{
    public interface IItemRepository
    {
        Task<IItem> GetByIdAsync(Guid id);
        Task<IReadOnlyList<IItem>> GetAllAsync();
        Task<IReadOnlyList<IItem>> GetByTypeAsync(string type);
        Task<Guid> AddAsync(IItem item);
        Task UpdateAsync(IItem item);
        Task DeleteAsync(Guid id);
    }
}