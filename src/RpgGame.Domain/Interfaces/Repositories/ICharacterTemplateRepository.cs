using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RpgGame.Domain.Common;
using RpgGame.Domain.Entities.Configuration;
using RpgGame.Domain.Enums;

namespace RpgGame.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for managing character templates.
    /// </summary>
    public interface ICharacterTemplateRepository : IRepository<CharacterTemplate>
    {
        #region Read Operations

        Task<OperationResult<IEnumerable<CharacterTemplate>>> GetByCharacterTypeAsync(CharacterType characterType);
        Task<OperationResult<IEnumerable<CharacterTemplate>>> GetByNPCBehaviorAsync(NPCBehavior behavior);
        Task<OperationResult<IEnumerable<CharacterTemplate>>> GetByPlayerClassAsync(PlayerClass playerClass);
        Task<OperationResult<CharacterTemplate>> GetByNameAsync(string name);

        #endregion

        #region Write Operations

        new Task<OperationResult<CharacterTemplate>> AddAsync(CharacterTemplate characterTemplate);
        new Task<OperationResult<CharacterTemplate>> UpdateAsync(CharacterTemplate characterTemplate);
        new Task<OperationResult> DeleteAsync(CharacterTemplate characterTemplate);
        Task<OperationResult> DeleteAsync(Guid id);

        #endregion

    }
}