using RpgGame.Domain.Enums;
using System;

namespace RpgGame.Application.Serialization.DTOs
{
    [Serializable]
    public class ItemDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Value { get; set; }
        public ItemType Type { get; set; }

        // Base item properties
        public string ItemClass { get; set; } // The specific item class type (e.g., "HealthPotion", "Sword")

        // Equipment specific properties
        public EquipmentSlot? Slot { get; set; }
        public int? BonusValue { get; set; }

        // Consumable specific properties
        public int? HealAmount { get; set; }
        public int? ManaAmount { get; set; }
    }
}