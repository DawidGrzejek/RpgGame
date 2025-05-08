using RpgGame.Domain.Enums;
using System;

namespace RpgGame.WebApi.DTOs.Characters
{
    public class CharacterDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Level { get; set; }
        public int Strength { get; set; }
        public int Defense { get; set; }
        public CharacterType CharacterType { get; set; }
        public int Experience { get; set; }
        public int ExperienceToNextLevel { get; set; }

        // Type-specific properties
        public double? CriticalChance { get; set; } // For Rogue
        public int? Mana { get; set; } // For Mage
        public int? MaxMana { get; set; } // For Mage
    }
}