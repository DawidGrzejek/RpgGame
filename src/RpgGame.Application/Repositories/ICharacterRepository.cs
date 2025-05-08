using RpgGame.Domain.Entities.Characters.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RpgGame.Application.Repositories
{
    public interface ICharacterRepository
    {
        Task<Character> GetByIdAsync(Guid id);
        Task<IReadOnlyList<Character>> GetAllAsync();
        Task<Guid> AddAsync(Character character);
        Task UpdateAsync(Character character);
        Task DeleteAsync(Guid id);
    }
}