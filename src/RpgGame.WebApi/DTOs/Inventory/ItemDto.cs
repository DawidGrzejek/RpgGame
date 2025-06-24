using RpgGame.Domain.Enums;
using System;

namespace RpgGame.WebApi.DTOs.Inventory
{
    public class ItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Value { get; set; }
        public ItemType Type { get; set; }

        // Equipment specific properties
        public EquipmentSlot? Slot { get; set; }
        public int? BonusValue { get; set; }

        // Consumable specific properties
        public int? HealAmount { get; set; }
        public int? ManaAmount { get; set; }

        public bool IsEquipment => Slot.HasValue;
        public bool IsConsumable => HealAmount.HasValue || ManaAmount.HasValue;
    }
}