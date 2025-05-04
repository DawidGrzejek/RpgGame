using RpgGame.Application.Serialization.DTOs;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Entities.Characters.Player;
using RpgGame.Domain.Events.Characters;
using RpgGame.Domain.Interfaces.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Application.Serialization.Mappers
{
    public static class PlayerMapper
    {
        public static PlayerCharacterDto ToDto(PlayerCharacter character)
        {
            var stateEvent = character.ExportState() as PlayerStateExported;

            return new PlayerCharacterDto
            {
                Name = character.Name,
                Health = character.Health,
                MaxHealth = character.MaxHealth,
                Level = character.Level,
                Strength = character.Strength,
                Defense = character.Defense,
                Experience = character.Experience,
                CharacterType = character.GetType().Name,
                Inventory = InventoryMapper.ToDto(character.Inventory),
                EquippedItems = stateEvent.EquippedItems.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Name),
                CriticalChance = (character as Rogue)?.CriticalChance,
                Mana = (character as Mage)?.Mana,
                MaxMana = (character as Mage)?.MaxMana
            };
        }

        public static PlayerCharacter FromDto(PlayerCharacterDto dto)
        {
            // Create inventory
            IInventory inventory = InventoryMapper.FromDto(dto.Inventory);

            // Create character based on type
            PlayerCharacter character = null;

            switch (dto.CharacterType)
            {
                case "Rogue":
                    character = Rogue.Create(dto.Name, inventory);
                    break;
                case "Mage":
                    character = Mage.Create(dto.Name, inventory);
                    break;
                case "Warrior":
                    character = Warrior.Create(dto.Name, inventory);
                    break;
                default:
                    throw new ArgumentException($"Unsupported character type: {dto.CharacterType}");
            }

            // Set common properties using reflection
            SetPrivateField(character, "_health", dto.Health);
            SetPrivateField(character, "_maxHealth", dto.MaxHealth);
            SetPrivateField(character, "_level", dto.Level);
            SetPrivateField(character, "_strength", dto.Strength);
            SetPrivateField(character, "_defense", dto.Defense);
            SetPrivateField(character, "_experience", dto.Experience);

            // Set type-specific properties
            if (character is Rogue rogue && dto.CriticalChance.HasValue)
            {
                SetPrivateField(rogue, "_criticalChance", dto.CriticalChance.Value);
            }
            else if (character is Mage mage && dto.Mana.HasValue && dto.MaxMana.HasValue)
            {
                SetPrivateField(mage, "_mana", dto.Mana.Value);
                SetPrivateField(mage, "_maxMana", dto.MaxMana.Value);
            }

            // Handle equipped items (this would need further implementation)
            // Code for restoring equipped items would go here

            return character;
        }

        private static void SetPrivateField(object obj, string fieldName, object value)
        {
            var field = obj.GetType().GetField(fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            if (field != null)
            {
                field.SetValue(obj, value);
            }
            else
            {
                // Try to find the field in the base class
                Type baseType = obj.GetType().BaseType;
                while (baseType != null)
                {
                    field = baseType.GetField(fieldName,
                        BindingFlags.Instance | BindingFlags.NonPublic);

                    if (field != null)
                    {
                        field.SetValue(obj, value);
                        return;
                    }

                    baseType = baseType.BaseType;
                }
            }
        }
    }
}
