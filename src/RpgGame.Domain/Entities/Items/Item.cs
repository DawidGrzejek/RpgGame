using System;
using System.Collections.Generic;
using System.Linq;
using RpgGame.Domain.Base;
using RpgGame.Domain.Entities.Configuration;
using RpgGame.Domain.Enums;
using RpgGame.Domain.Interfaces.Items;
using RpgGame.Domain.Interfaces.Characters;

namespace RpgGame.Domain.Entities.Items
{
    /// <summary>
    /// Unified item entity that uses composition instead of inheritance
    /// This replaces the complex item hierarchy with template-driven design
    /// </summary>
    public class Item : DomainEntity, IItem, IEquipment
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public ItemType Type { get; private set; }
        public int Value { get; private set; }
        public bool IsStackable { get; private set; }
        public int StackSize { get; private set; }
        
        // Template reference for configuration
        public Guid TemplateId { get; private set; }
        
        // Item-specific data (replaces hard-coded properties)
        public Dictionary<string, object> CustomData { get; private set; }
        
        // For equipable items (IEquipment interface)
        public bool IsEquipped { get; private set; }
        public EquipmentSlot? EquipmentSlot { get; private set; }
        public Dictionary<string, int> StatModifiers { get; private set; }
        
        // IEquipment interface properties
        public EquipmentSlot Slot => EquipmentSlot ?? Enums.EquipmentSlot.None;
        public int BonusValue => StatModifiers.Values.Sum();
        
        // For consumable items
        public bool IsConsumable { get; private set; }
        public Dictionary<string, object> ConsumableEffects { get; private set; }

        // Private constructor for EF Core
        private Item()
        {
            CustomData = new Dictionary<string, object>();
            StatModifiers = new Dictionary<string, int>();
            ConsumableEffects = new Dictionary<string, object>();
        }

        // Factory method to create from template
        public static Item CreateFromTemplate(ItemTemplate template, int stackSize = 1)
        {
            var item = new Item
            {
                Id = Guid.NewGuid(),
                Name = template.Name,
                Description = template.Description,
                Type = template.ItemType,
                Value = template.Value,
                TemplateId = template.Id,
                IsConsumable = template.IsConsumable,
                IsStackable = template.ItemType == ItemType.Potion || template.ItemType == ItemType.Miscellaneous,
                StackSize = stackSize,
                EquipmentSlot = template.EquipmentSlot,
                CustomData = new Dictionary<string, object>(),
                StatModifiers = new Dictionary<string, int>(template.StatModifiers),
                ConsumableEffects = new Dictionary<string, object>()
            };

            return item;
        }

        // Factory method for direct creation (for testing/seeding)
        public static Item Create(
            string name,
            string description,
            ItemType type,
            int value,
            bool isConsumable = false,
            EquipmentSlot? equipmentSlot = null)
        {
            return new Item
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                Type = type,
                Value = value,
                IsConsumable = isConsumable,
                IsStackable = type == ItemType.Potion || type == ItemType.Miscellaneous,
                StackSize = 1,
                EquipmentSlot = equipmentSlot,
                TemplateId = Guid.Empty, // No template used
                CustomData = new Dictionary<string, object>(),
                StatModifiers = new Dictionary<string, int>(),
                ConsumableEffects = new Dictionary<string, object>()
            };
        }

        // IEquipment interface methods
        public void Equip()
        {
            if (!CanBeEquipped())
                throw new InvalidOperationException($"Item {Name} cannot be equipped");
            
            IsEquipped = true;
        }

        public void Unequip()
        {
            IsEquipped = false;
        }

        public bool CanBeEquipped()
        {
            return EquipmentSlot.HasValue && Type != ItemType.Potion;
        }

        // IEquipment interface methods
        public void OnEquip(IPlayerCharacter character)
        {
            if (!CanBeEquipped())
                throw new InvalidOperationException($"Item {Name} cannot be equipped");
            
            IsEquipped = true;
            // Apply stat modifiers to character (would be implemented based on character system)
        }

        public void OnUnequip(IPlayerCharacter character)
        {
            IsEquipped = false;
            // Remove stat modifiers from character (would be implemented based on character system)
        }

        // Item usage for consumables
        public void Use()
        {
            if (!IsConsumable)
                throw new InvalidOperationException($"Item {Name} is not consumable");
            
            // Apply consumable effects (this would be handled by item use handlers)
            StackSize = Math.Max(0, StackSize - 1);
        }

        // Template application
        public void ApplyTemplate(ItemTemplate template)
        {
            TemplateId = template.Id;
            
            // Update properties from template
            Name = template.Name;
            Description = template.Description;
            Type = template.ItemType;
            Value = template.Value;
            IsConsumable = template.IsConsumable;
            EquipmentSlot = template.EquipmentSlot;
            
            // Apply stat modifiers
            StatModifiers.Clear();
            foreach (var modifier in template.StatModifiers)
            {
                StatModifiers[modifier.Key] = modifier.Value;
            }
        }

        // Add custom data (for template-driven behavior)
        public void SetCustomData(string key, object value)
        {
            CustomData[key] = value;
        }

        public T GetCustomData<T>(string key, T defaultValue = default)
        {
            if (CustomData.TryGetValue(key, out var value))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            return defaultValue;
        }

        // Add stat modifier
        public void AddStatModifier(string statName, int modifier)
        {
            StatModifiers[statName] = modifier;
        }

        // Update stack size
        public void UpdateStackSize(int newSize)
        {
            if (!IsStackable)
                throw new InvalidOperationException($"Item {Name} is not stackable");
            
            StackSize = Math.Max(0, newSize);
        }

        public override string ToString()
        {
            var stackInfo = IsStackable && StackSize > 1 ? $" x{StackSize}" : "";
            var equipInfo = IsEquipped ? " (Equipped)" : "";
            return $"{Name}{stackInfo}{equipInfo} - {Description}";
        }
    }
}