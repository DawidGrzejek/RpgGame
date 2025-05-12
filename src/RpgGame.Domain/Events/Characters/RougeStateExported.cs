using RpgGame.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Domain.Events.Characters
{
    public class RougeStateExported : PlayerStateExported
    {
        public double CriticalChance { get; }

        public RougeStateExported(
            Guid aggregateId,
            int version,
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
            double criticalChance)
            : base(aggregateId, version, name, health, maxHealth, level, strength, defense, isAlive, characterType, experience, experienceToNextLevel, equippedItems, inventory, gold)
        {
            CriticalChance = criticalChance;
        }
    }
}
