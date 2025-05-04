using RpgGame.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Domain.Events.Characters
{
    /// <summary>
    /// Raised when a player's state is exported.
    /// </summary>
    public class PlayerStateExported : CharacterStateExported
    {
        public int Experience { get; }
        public int ExperienceToNextLevel { get; }
        public Dictionary<EquipmentSlot, string> EquippedItems { get; }
        public List<string> Inventory { get; }
        public int Gold { get; }

        public PlayerStateExported(
            string name,
            int health,
            int maxHealth,
            int level,
            int strength,
            int defense,
            int isAlive,
            string characterType,
            int experience,
            int experienceToNextLevel,
            Dictionary<EquipmentSlot, string> equippedItems,
            List<string> inventory,
            int gold)
            : base(name, health, maxHealth, level, strength, defense, isAlive, characterType)
        {
            Experience = experience;
            ExperienceToNextLevel = experienceToNextLevel;
            EquippedItems = equippedItems;
            Inventory = inventory;
            Gold = gold;
        }
    }
}
