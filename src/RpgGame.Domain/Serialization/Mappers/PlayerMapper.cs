using RpgGame.Application.Serialization.DTOs;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Events.Characters;
using RpgGame.Domain.Interfaces.Inventory;
using RpgGame.Domain.Enums;
using RpgGame.Domain.ValueObjects;
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
        public static PlayerCharacterDto ToDto(Character character)
        {
            if (character.Type != CharacterType.Player)
            {
                throw new InvalidOperationException("Character must be a player character.");
            }

            var stateEvent = character.ExportState();

            return new PlayerCharacterDto
            {
                Name = character.Name,
                Health = character.Stats.CurrentHealth,
                MaxHealth = character.Stats.MaxHealth,
                Level = character.Stats.Level,
                Strength = character.Stats.Strength,
                Defense = character.Stats.Defense,
                Experience = character.Experience,
                CharacterType = character.PlayerClass?.ToString() ?? "Unknown",
                Inventory = null, // TODO: Implement inventory mapping for new system
                EquippedItems = new Dictionary<string, object>(), // TODO: Implement equipment mapping
                
                // Type-specific properties from CustomData
                CriticalChance = character.CustomData.TryGetValue("CriticalChance", out var crit) ? (double?)crit : null,
                Mana = character.CustomData.TryGetValue("Mana", out var mana) ? (int?)mana : null,
                MaxMana = character.CustomData.TryGetValue("MaxMana", out var maxMana) ? (int?)maxMana : null
            };
        }

        public static Character FromDto(PlayerCharacterDto dto)
        {
            // Parse player class from string
            if (!Enum.TryParse<PlayerClass>(dto.CharacterType, out var playerClass))
            {
                throw new ArgumentException($"Unsupported character type: {dto.CharacterType}");
            }

            // Create base stats from DTO
            var stats = new CharacterStats(
                dto.Level,
                dto.MaxHealth,
                dto.Strength,
                dto.Defense,
                10, // Default speed - TODO: add to DTO if needed
                5   // Default magic - TODO: add to DTO if needed
            );

            // Set current health
            stats = stats with { CurrentHealth = dto.Health };

            // Create character using our new factory methods
            var character = Character.CreatePlayer(dto.Name, playerClass, stats);
            
            // Set experience directly (bypassing level-up checks since we're restoring state)
            typeof(Character).GetField("Experience", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(character, dto.Experience);

            // Add type-specific data to CustomData
            if (dto.CriticalChance.HasValue)
            {
                character.CustomData["CriticalChance"] = dto.CriticalChance.Value;
            }
            if (dto.Mana.HasValue)
            {
                character.CustomData["Mana"] = dto.Mana.Value;
            }
            if (dto.MaxMana.HasValue)
            {
                character.CustomData["MaxMana"] = dto.MaxMana.Value;
            }

            // TODO: Restore inventory and equipped items when inventory system is updated

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
