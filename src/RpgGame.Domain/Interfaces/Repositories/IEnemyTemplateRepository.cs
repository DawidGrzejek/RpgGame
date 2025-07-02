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
    /// Repository interface for EnemyTemplate entities
    /// Inherits basic CRUD + adds enemy-specific operations
    /// </summary>
    public interface IEnemyTemplateRepository : IRepository<EnemyTemplate>
    {
        Task<OperationResult<IEnumerable<EnemyTemplate>>> GetByTypeAsync(EnemyType enemyType);
        Task<OperationResult<IEnumerable<EnemyTemplate>>> GetByNameAsync(string name);
        Task<OperationResult<bool>> ExistsByNameAsync(string name);
        Task<OperationResult<IEnumerable<EnemyTemplate>>> GetByExperienceRangeAsync(int minExp, int maxExp);
        Task<OperationResult<IEnumerable<EnemyTemplate>>> GetByHealthRangeAsync(int minHealth, int maxHealth);
    }
}