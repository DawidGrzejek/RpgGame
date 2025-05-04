using RpgGame.Domain.Events.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Domain.Events.Characters
{
    /// <summary>
    /// Raised when a character's state is exported.
    /// </summary>
    public class CharacterStateExported : DomainEventBase
    {
        public string Name { get; }
        public int Health { get; }
        public int MaxHealth { get; }
        public int Level { get; }
        public int Strength { get; }
        public int Defense { get; }
        public int IsAlive { get; }
        public string CharacterType { get; }

        public CharacterStateExported(string name, int health, int maxHealth, int level, int strength, int defense, int isAlive, string characterType)
        {
            Name = name;
            Health = health;
            MaxHealth = maxHealth;
            Level = level;
            Strength = strength;
            Defense = defense;
            IsAlive = isAlive;
            CharacterType = characterType;
        }
    }
}
