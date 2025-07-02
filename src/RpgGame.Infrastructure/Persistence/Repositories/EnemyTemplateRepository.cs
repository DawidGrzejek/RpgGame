using Microsoft.EntityFrameworkCore;
using RpgGame.Domain.Common;
using RpgGame.Domain.Entities.Configuration;
using RpgGame.Domain.Enums;
using RpgGame.Domain.Interfaces.Repositories;
using RpgGame.Infrastructure.Persistence.EFCore;

namespace RpgGame.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Concrete implementation of EnemyTemplate repository using Entity Framework
    /// </summary>
    public class EnemyTemplateRepository : Repository<EnemyTemplate>, IEnemyTemplateRepository
    {
        public EnemyTemplateRepository(GameDbContext context) : base(context)
        {
        }

        public async Task<OperationResult<IEnumerable<EnemyTemplate>>> GetByTypeAsync(EnemyType enemyType)
        {
            try
            {
                var result = await _dbSet
                    .Where(e => e.EnemyType == enemyType)
                    .OrderBy(e => e.Name)
                    .ToListAsync();
                return OperationResult<IEnumerable<EnemyTemplate>>.Success(result);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<EnemyTemplate>>.Failure(new OperationError("Failed to get enemies by type.", ex.Message, ex.InnerException?.Message ?? string.Empty));
            }
        }

        public async Task<OperationResult<IEnumerable<EnemyTemplate>>> GetByNameAsync(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return OperationResult<IEnumerable<EnemyTemplate>>.Success(Enumerable.Empty<EnemyTemplate>());

                var result = await _dbSet
                    .Where(e => e.Name.Contains(name))
                    .OrderBy(e => e.Name)
                    .ToListAsync();
                return OperationResult<IEnumerable<EnemyTemplate>>.Success(result);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<EnemyTemplate>>.Failure(new OperationError("Failed to get enemies by name.", ex.Message, ex.InnerException?.Message ?? string.Empty));
            }
        }

        public async Task<OperationResult<bool>> ExistsByNameAsync(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return OperationResult<bool>.Success(false);

                var exists = await _dbSet
                    .AnyAsync(e => e.Name.ToLower() == name.ToLower());
                return OperationResult<bool>.Success(exists);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Failure(new OperationError("Failed to check if enemy exists by name.", ex.Message, ex.InnerException?.Message ?? string.Empty));
            }
        }

        public async Task<OperationResult<IEnumerable<EnemyTemplate>>> GetByExperienceRangeAsync(int minExp, int maxExp)
        {
            try
            {
                var result = await _dbSet
                    .Where(e => e.ExperienceReward >= minExp && e.ExperienceReward <= maxExp)
                    .OrderBy(e => e.ExperienceReward)
                    .ToListAsync();
                return OperationResult<IEnumerable<EnemyTemplate>>.Success(result);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<EnemyTemplate>>.Failure(new OperationError("Failed to get enemies by experience range.", ex.Message, ex.InnerException?.Message ?? string.Empty));
            }
        }

        public async Task<OperationResult<IEnumerable<EnemyTemplate>>> GetByHealthRangeAsync(int minHealth, int maxHealth)
        {
            try
            {
                var result = await _dbSet
                    .Where(e => e.BaseHealth >= minHealth && e.BaseHealth <= maxHealth)
                    .OrderBy(e => e.BaseHealth)
                    .ToListAsync();
                return OperationResult<IEnumerable<EnemyTemplate>>.Success(result);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<EnemyTemplate>>.Failure(new OperationError("Failed to get enemies by health range.", ex.Message, ex.InnerException?.Message ?? string.Empty));
            }
        }
    }
}