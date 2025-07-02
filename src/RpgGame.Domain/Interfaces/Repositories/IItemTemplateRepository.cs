using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RpgGame.Domain.Common;
using RpgGame.Domain.Entities.Configuration;
using RpgGame.Domain.Enums;

namespace RpgGame.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for ItemTemplate entities
    /// Inherits basic CRUD + adds item-specific operations
    /// </summary>
    public interface IItemTemplateRepository : IRepository<ItemTemplate>
    {
        Task<OperationResult<IEnumerable<ItemTemplate>>> GetByTypeAsync(ItemType itemType);
        Task<OperationResult<IEnumerable<ItemTemplate>>> GetByNameAsync(string name);
        Task<OperationResult<IEnumerable<ItemTemplate>>> GetEquippableItemsAsync();
        Task<OperationResult<IEnumerable<ItemTemplate>>> GetConsumableItemsAsync();
        Task<OperationResult<IEnumerable<ItemTemplate>>> GetByEquipmentSlotAsync(EquipmentSlot slot);
        Task<OperationResult<IEnumerable<ItemTemplate>>> GetByValueRangeAsync(int minValue, int maxValue);
        Task<OperationResult<bool>> ExistsByNameAsync(string name);
    }
}