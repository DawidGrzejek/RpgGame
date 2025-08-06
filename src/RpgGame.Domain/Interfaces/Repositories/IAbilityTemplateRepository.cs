using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RpgGame.Domain.Entities.Configuration;
using RpgGame.Domain.Enums;

namespace RpgGame.Domain.Interfaces.Repositories
{
    public interface IAbilityTemplateRepository : IRepository<AbilityTemplate>
    {
        Task<IEnumerable<AbilityTemplate>> GetByAbilityTypeAsync(AbilityType abilityType);
        Task<IEnumerable<AbilityTemplate>> GetByTargetTypeAsync(TargetType targetType);
        Task<AbilityTemplate> GetByNameAsync(string name);
        Task<IEnumerable<AbilityTemplate>> GetAvailableForCharacterAsync(Dictionary<string, object> characterData);
    }
}