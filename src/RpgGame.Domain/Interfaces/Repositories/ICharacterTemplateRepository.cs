using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RpgGame.Domain.Entities.Configuration;
using RpgGame.Domain.Enums;

namespace RpgGame.Domain.Interfaces.Repositories
{
    public interface ICharacterTemplateRepository : IRepository<CharacterTemplate>
    {
        Task<IEnumerable<CharacterTemplate>> GetByCharacterTypeAsync(CharacterType characterType);
        Task<IEnumerable<CharacterTemplate>> GetByNPCBehaviorAsync(NPCBehavior behavior);
        Task<IEnumerable<CharacterTemplate>> GetByPlayerClassAsync(PlayerClass playerClass);
        Task<CharacterTemplate> GetByNameAsync(string name);
    }
}