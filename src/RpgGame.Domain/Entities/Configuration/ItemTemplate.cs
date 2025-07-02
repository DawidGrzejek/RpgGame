using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RpgGame.Domain.Base;
using RpgGame.Domain.Enums;

namespace RpgGame.Domain.Entities.Configuration
{
    public class ItemTemplate : DomainEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public ItemType ItemType { get; private set; }
        public int Value { get; private set; }
        public Dictionary<string, int> StatModifiers { get; private set; }
        public bool IsConsumable { get; private set; }
        public bool IsEquippable { get; private set; }
        public EquipmentSlot? EquipmentSlot { get; private set; }

        public ItemTemplate(
            string name,
            string description,
            ItemType itemType,
            int value,
            bool isConsumable = false,
            bool isEquippable = false,
            EquipmentSlot? equipmentSlot = null)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            ItemType = itemType;
            Value = value;
            IsConsumable = isConsumable;
            IsEquippable = isEquippable;
            EquipmentSlot = equipmentSlot;
            StatModifiers = new Dictionary<string, int>();
        }

        public void AddStatModifier(string statName, int modifier)
        {
            StatModifiers[statName] = modifier;
        }

        public void UpdateDetails(string name, string description, ItemType itemType, int value, bool isConsumable, bool isEquippable, EquipmentSlot? equipmentSlot)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            ItemType = itemType;
            Value = value;
            IsConsumable = isConsumable;
            IsEquippable = isEquippable;
            EquipmentSlot = equipmentSlot;
        }

        public void ClearStatModifiers()
        {
            StatModifiers.Clear();
        }
    }
}