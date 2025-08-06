using System;

namespace RpgGame.Domain.ValueObjects
{
    /// <summary>
    /// Immutable value object representing character statistics
    /// </summary>
    public record CharacterStats
    {
        public int Level { get; init; }
        public int CurrentHealth { get; init; }
        public int MaxHealth { get; init; }
        public int Strength { get; init; }
        public int Defense { get; init; }
        public int Speed { get; init; }
        public int Magic { get; init; }

        public CharacterStats(
            int level = 1,
            int maxHealth = 100,
            int strength = 10,
            int defense = 5,
            int speed = 10,
            int magic = 5)
        {
            if (level < 1) throw new ArgumentException("Level must be at least 1");
            if (maxHealth < 1) throw new ArgumentException("Max health must be at least 1");
            
            Level = level;
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth; // Start at full health
            Strength = Math.Max(1, strength);
            Defense = Math.Max(0, defense);
            Speed = Math.Max(1, speed);
            Magic = Math.Max(0, magic);
        }

        // Immutable updates
        public CharacterStats WithHealth(int newHealth) => 
            this with { CurrentHealth = Math.Max(0, Math.Min(MaxHealth, newHealth)) };

        public CharacterStats LevelUp() => 
            this with 
            { 
                Level = Level + 1,
                MaxHealth = MaxHealth + 10,
                CurrentHealth = MaxHealth + 10, // Full heal on level up
                Strength = Strength + 2,
                Defense = Defense + 1,
                Speed = Speed + 1,
                Magic = Magic + 1
            };

        public CharacterStats WithModifiers(
            int strengthMod = 0,
            int defenseMod = 0,
            int speedMod = 0,
            int magicMod = 0) =>
            this with
            {
                Strength = Math.Max(1, Strength + strengthMod),
                Defense = Math.Max(0, Defense + defenseMod),
                Speed = Math.Max(1, Speed + speedMod),
                Magic = Math.Max(0, Magic + magicMod)
            };
    }
}