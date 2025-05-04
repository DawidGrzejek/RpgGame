using RpgGame.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Domain.Events.Characters
{
    public class MageStateExported : PlayerStateExported
    {
        public int Mana { get; }
        public int MaxMana { get; }

        public MageStateExported(
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
            int gold,
            int mana,
            int maxMana)
            : base(name, health, maxHealth, level, strength, defense, isAlive, characterType, experience, experienceToNextLevel, equippedItems, inventory, gold)
        {
            Mana = mana;
            MaxMana = maxMana;
        }
    }
}
