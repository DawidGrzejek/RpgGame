using RpgGame.Domain.Interfaces.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Application.Repositories
{
    public class ItemRepository : IItemRepository
    {
        public Task<Guid> AddAsync(IItem item)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<IItem>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IItem> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<IItem>> GetByTypeAsync(string type)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(IItem item)
        {
            throw new NotImplementedException();
        }
    }
}
