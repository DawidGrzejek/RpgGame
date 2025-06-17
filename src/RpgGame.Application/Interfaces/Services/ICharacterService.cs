using RpgGame.Domain.Common;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Application.Interfaces.Services
{
    public interface ICharacterService
    {
        Task<OperationResult<Character>> CreateCharacterAsync(string name, CharacterType type);
        Task<OperationResult> LevelUpCharacterAsync(Guid characterId);
    }
}
