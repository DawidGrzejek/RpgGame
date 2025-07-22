using RpgGame.Application.Interfaces.Repositories;
using RpgGame.Application.Interfaces.Services;
using RpgGame.Domain.Common;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Entities.Characters.Player;
using RpgGame.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Application.Services
{
    public class CharacterService : ICharacterService
    {
        private readonly ICharacterRepository _characterRepository;

        public CharacterService(ICharacterRepository characterRepository)
        {
            _characterRepository = characterRepository ?? throw new ArgumentNullException(nameof(characterRepository));
        }

        public async Task<OperationResult<Character>> CreateCharacterAsync(string name, CharacterType type)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(name))
                return OperationResult<Character>.ValidationFailed(nameof(name), "Character name is required");

            if (name.Length < 3)
                return OperationResult<Character>.ValidationFailed(nameof(name), "Character name must be at least 3 characters");

            try
            {
                // Create character
                Character character = type switch
                {
                    CharacterType.Warrior => Warrior.Create(name),
                    CharacterType.Mage => Mage.Create(name),
                    CharacterType.Rogue => Rogue.Create(name),
                    _ => throw new ArgumentException("Invalid character type")
                };

                // Save to repository
                var saveResult = await _characterRepository.AddAsync(character);
                if (!saveResult.Succeeded)
                    return OperationResult<Character>.Failure(saveResult.Errors);

                return OperationResult<Character>.Success(character);
            }
            catch (Exception ex)
            {
                return OperationResult<Character>.Failure("Character.CreationFailed", $"Failed to create character: {ex.Message}");
            }
        }

        public async Task<OperationResult> LevelUpCharacterAsync(Guid characterId)
        {
            var getResult = await _characterRepository.GetByIdAsync(characterId);
            if (!getResult.Succeeded)
                return OperationResult.Failure(getResult.Errors);

            var character = getResult.Data;
            if (character == null)
                return OperationResult.NotFound("Character", characterId.ToString());

            if (!character.IsAlive)
                return OperationResult.BusinessRuleViolation("DeadCharacter", "Cannot level up a dead character");

            character.LevelUp();

            var updateResult = await _characterRepository.UpdateAsync(character);
            return updateResult.Succeeded
                ? OperationResult.Success()
                : OperationResult.Failure(updateResult.Errors);
        }
    }
}
