using Microsoft.EntityFrameworkCore;
using RpgGame.Domain.Common;
using RpgGame.Domain.Entities.Configuration;
using RpgGame.Domain.Enums;
using RpgGame.Domain.Interfaces.Repositories;
using RpgGame.Infrastructure.Persistence.EFCore;

namespace RpgGame.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Concrete implementation of ItemTemplate repository using Entity Framework
    /// </summary>
    public class ItemTemplateRepository : Repository<ItemTemplate>, IItemTemplateRepository
    {
        public ItemTemplateRepository(GameDbContext context) : base(context)
        {
        }

        public async Task<OperationResult<IEnumerable<ItemTemplate>>> GetByTypeAsync(ItemType itemType)
        {
            try
            {
                var items = await _dbSet
                    .Where(i => i.ItemType == itemType)
                    .OrderBy(i => i.Name)
                    .ToListAsync();
                return OperationResult<IEnumerable<ItemTemplate>>.Success(items);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<ItemTemplate>>.Failure(new OperationError("Failed to get items by type.", ex.Message, ex.InnerException?.Message ?? string.Empty));
            }
        }

        public async Task<OperationResult<IEnumerable<ItemTemplate>>> GetByNameAsync(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return OperationResult<IEnumerable<ItemTemplate>>.Success(Enumerable.Empty<ItemTemplate>());

                var items = await _dbSet
                    .Where(i => i.Name.Contains(name))
                    .OrderBy(i => i.Name)
                    .ToListAsync();
                return OperationResult<IEnumerable<ItemTemplate>>.Success(items);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<ItemTemplate>>.Failure(new OperationError("Failed to get items by name.", ex.Message, ex.InnerException?.Message ?? string.Empty));
            }
        }

        public async Task<OperationResult<IEnumerable<ItemTemplate>>> GetEquippableItemsAsync()
        {
            try
            {
                var items = await _dbSet
                    .Where(i => i.IsEquippable)
                    .OrderBy(i => i.Name)
                    .ToListAsync();
                return OperationResult<IEnumerable<ItemTemplate>>.Success(items);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<ItemTemplate>>.Failure(new OperationError("Failed to get equippable items.", ex.Message, ex.InnerException?.Message ?? string.Empty));
            }
        }

        public async Task<OperationResult<IEnumerable<ItemTemplate>>> GetConsumableItemsAsync()
        {
            try
            {
                var items = await _dbSet
                    .Where(i => i.IsConsumable)
                    .OrderBy(i => i.Name)
                    .ToListAsync();
                return OperationResult<IEnumerable<ItemTemplate>>.Success(items);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<ItemTemplate>>.Failure(new OperationError("Failed to get consumable items.", ex.Message, ex.InnerException?.Message ?? string.Empty));
            }
        }

        public async Task<OperationResult<IEnumerable<ItemTemplate>>> GetByEquipmentSlotAsync(EquipmentSlot slot)
        {
            try
            {
                var items = await _dbSet
                    .Where(i => i.IsEquippable && i.EquipmentSlot == slot)
                    .OrderBy(i => i.Name)
                    .ToListAsync();
                return OperationResult<IEnumerable<ItemTemplate>>.Success(items);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<ItemTemplate>>.Failure(new OperationError("Failed to get items by equipment slot.", ex.Message, ex.InnerException?.Message ?? string.Empty));
            }
        }

        public async Task<OperationResult<IEnumerable<ItemTemplate>>> GetByValueRangeAsync(int minValue, int maxValue)
        {
            try
            {
                var items = await _dbSet
                    .Where(i => i.Value >= minValue && i.Value <= maxValue)
                    .OrderBy(i => i.Value)
                    .ToListAsync();
                return OperationResult<IEnumerable<ItemTemplate>>.Success(items);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<ItemTemplate>>.Failure(new OperationError("Failed to get items by value range.", ex.Message, ex.InnerException?.Message ?? string.Empty));
            }
        }

        public async Task<OperationResult<bool>> ExistsByNameAsync(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return OperationResult<bool>.Success(false);

                var exists = await _dbSet
                    .AnyAsync(i => i.Name.ToLower() == name.ToLower());
                return OperationResult<bool>.Success(exists);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Failure(new OperationError("Failed to check if item exists by name.", ex.Message, ex.InnerException?.Message ?? string.Empty));
            }
        }
    }
}