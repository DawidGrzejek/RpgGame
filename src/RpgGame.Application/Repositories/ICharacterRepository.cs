using RpgGame.Domain.Common;
using RpgGame.Domain.Entities.Characters.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RpgGame.Application.Repositories
{
    public interface ICharacterRepository
    {
        Task<OperationResult<Character>> GetByIdAsync(Guid id);
        Task<OperationResult<IReadOnlyList<Character>>> GetAllAsync();
        Task<OperationResult<Guid>> AddAsync(Character character);
        Task<OperationResult> UpdateAsync(Character character);
        Task<OperationResult> DeleteAsync(Guid id);
    }
}