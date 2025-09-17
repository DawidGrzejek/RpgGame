using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RpgGame.Domain.Common;
using RpgGame.Domain.Entities.Configuration;
using RpgGame.Domain.Enums;

namespace RpgGame.Domain.Interfaces.Repositories
{
    public interface IAbilityTemplateRepository : IRepository<AbilityTemplate>
    {
        #region Read Operations

        Task<OperationResult<IEnumerable<AbilityTemplate>>> GetByAbilityTypeAsync(AbilityType abilityType);
        Task<OperationResult<IEnumerable<AbilityTemplate>>> GetByTargetTypeAsync(TargetType targetType);
        Task<OperationResult<AbilityTemplate>> GetByNameAsync(string name);
        Task<OperationResult<IEnumerable<AbilityTemplate>>> GetAvailableForCharacterAsync(Dictionary<string, object> characterData);

        #endregion

        #region Write Operations

        new Task<OperationResult<AbilityTemplate>> AddAsync(AbilityTemplate abilityTemplate);
        new Task<OperationResult<AbilityTemplate>> UpdateAsync(AbilityTemplate abilityTemplate);
        new Task<OperationResult> DeleteAsync(AbilityTemplate abilityTemplate);
        Task<OperationResult> DeleteAsync(Guid id);

        #endregion
    }
}