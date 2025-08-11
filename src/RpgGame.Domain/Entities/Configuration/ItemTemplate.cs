using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RpgGame.Domain.Base;
using RpgGame.Domain.Enums;

namespace RpgGame.Domain.Entities.Configuration
{
    /// <summary>
    /// Template for creating items with configurable properties
    /// This enables database-driven item creation without hard-coded item classes
    /// </summary>
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

        // Private constructor for EF Core
        private ItemTemplate()
        {
            StatModifiers = new Dictionary<string, int>();
        }

        /// <summary>
        /// Creates a new item template
        /// </summary>
        /// <param name="name">Name of the item</param>
        /// <param name="description">Description of the item</param>
        /// <param name="itemType">Type of the item</param>
        /// <param name="value">Base value of the item</param>
        /// <param name="isConsumable">Whether the item can be consumed</param>
        /// <param name="isEquippable">Whether the item can be equipped</param>
        /// <param name="equipmentSlot">Equipment slot for equippable items</param>
        /// <exception cref="ArgumentException">Thrown when name or description is null, empty, or whitespace</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is negative</exception>
        public ItemTemplate(
            string name,
            string description,
            ItemType itemType,
            int value,
            bool isConsumable = false,
            bool isEquippable = false,
            EquipmentSlot? equipmentSlot = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null, empty, or whitespace", nameof(name));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be null, empty, or whitespace", nameof(description));
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be negative");

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

        // 🎯 Template-Driven Design: Type-Safe Factory Methods

        /// <summary>
        /// Creates a weapon template with stat modifiers
        /// </summary>
        /// <param name="name">Name of the weapon</param>
        /// <param name="description">Description of the weapon</param>
        /// <param name="value">Base value of the weapon</param>
        /// <param name="slot">Equipment slot for the weapon</param>
        /// <param name="statModifiers">Optional stat modifiers for the weapon</param>
        /// <returns>A new weapon ItemTemplate</returns>
        /// <exception cref="ArgumentException">Thrown when name or description is null, empty, or whitespace</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is negative</exception>
        public static ItemTemplate CreateWeaponTemplate(
            string name,
            string description,
            int value,
            EquipmentSlot slot = Enums.EquipmentSlot.MainHand,
            Dictionary<string, int>? statModifiers = null)
        {
            var template = new ItemTemplate(name, description, ItemType.Weapon, value, false, true, slot);
            
            if (statModifiers != null)
            {
                foreach (var modifier in statModifiers)
                {
                    template.AddStatModifier(modifier.Key, modifier.Value);
                }
            }
            
            return template;
        }

        /// <summary>
        /// Creates an armor template with defensive bonuses
        /// </summary>
        /// <param name="name">Name of the armor</param>
        /// <param name="description">Description of the armor</param>
        /// <param name="value">Base value of the armor</param>
        /// <param name="slot">Equipment slot for the armor</param>
        /// <param name="statModifiers">Optional stat modifiers for the armor</param>
        /// <returns>A new armor ItemTemplate</returns>
        /// <exception cref="ArgumentException">Thrown when name or description is null, empty, or whitespace</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is negative</exception>
        public static ItemTemplate CreateArmorTemplate(
            string name,
            string description,
            int value,
            EquipmentSlot slot,
            Dictionary<string, int>? statModifiers = null)
        {
            var template = new ItemTemplate(name, description, ItemType.Armor, value, false, true, slot);
            
            if (statModifiers != null)
            {
                foreach (var modifier in statModifiers)
                {
                    template.AddStatModifier(modifier.Key, modifier.Value);
                }
            }
            
            return template;
        }

        /// <summary>
        /// Creates a consumable potion template
        /// </summary>
        /// <param name="name">Name of the potion</param>
        /// <param name="description">Description of the potion</param>
        /// <param name="value">Base value of the potion</param>
        /// <param name="effects">Optional effects the potion provides</param>
        /// <returns>A new potion ItemTemplate</returns>
        /// <exception cref="ArgumentException">Thrown when name or description is null, empty, or whitespace</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is negative</exception>
        public static ItemTemplate CreatePotionTemplate(
            string name,
            string description,
            int value,
            Dictionary<string, int>? effects = null)
        {
            var template = new ItemTemplate(name, description, ItemType.Potion, value, true, false, null);
            
            if (effects != null)
            {
                foreach (var effect in effects)
                {
                    template.AddStatModifier(effect.Key, effect.Value);
                }
            }
            
            return template;
        }

        /// <summary>
        /// Creates a quest item template
        /// </summary>
        /// <param name="name">Name of the quest item</param>
        /// <param name="description">Description of the quest item</param>
        /// <param name="value">Base value of the quest item (typically 0)</param>
        /// <returns>A new quest item ItemTemplate</returns>
        /// <exception cref="ArgumentException">Thrown when name or description is null, empty, or whitespace</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is negative</exception>
        public static ItemTemplate CreateQuestItemTemplate(
            string name,
            string description,
            int value = 0)
        {
            return new ItemTemplate(name, description, ItemType.QuestItem, value, false, false, null);
        }

        /// <summary>
        /// Creates a miscellaneous item template
        /// </summary>
        /// <param name="name">Name of the miscellaneous item</param>
        /// <param name="description">Description of the miscellaneous item</param>
        /// <param name="value">Base value of the miscellaneous item</param>
        /// <param name="isConsumable">Whether the item can be consumed</param>
        /// <returns>A new miscellaneous ItemTemplate</returns>
        /// <exception cref="ArgumentException">Thrown when name or description is null, empty, or whitespace</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is negative</exception>
        public static ItemTemplate CreateMiscellaneousTemplate(
            string name,
            string description,
            int value,
            bool isConsumable = false)
        {
            return new ItemTemplate(name, description, ItemType.Miscellaneous, value, isConsumable, false, null);
        }

        /// <summary>
        /// Adds or updates a stat modifier for the item template
        /// </summary>
        /// <param name="statName">The name of the stat to modify</param>
        /// <param name="modifier">The modifier value to apply</param>
        /// <exception cref="ArgumentException">Thrown when statName is null, empty, or whitespace</exception>
        public void AddStatModifier(string statName, int modifier)
        {
            if (string.IsNullOrWhiteSpace(statName))
                throw new ArgumentException("Stat name cannot be null, empty, or whitespace", nameof(statName));
            
            StatModifiers[statName] = modifier;
        }

        /// <summary>
        /// Removes a stat modifier from the item template
        /// </summary>
        /// <param name="statName">The name of the stat modifier to remove</param>
        /// <returns>True if the modifier was found and removed, false otherwise</returns>
        /// <exception cref="ArgumentException">Thrown when statName is null, empty, or whitespace</exception>
        public bool RemoveStatModifier(string statName)
        {
            if (string.IsNullOrWhiteSpace(statName))
                throw new ArgumentException("Stat name cannot be null, empty, or whitespace", nameof(statName));
            
            return StatModifiers.Remove(statName);
        }

        /// <summary>
        /// Gets a stat modifier value by name
        /// </summary>
        /// <param name="statName">The name of the stat modifier</param>
        /// <param name="defaultValue">The default value to return if the modifier is not found</param>
        /// <returns>The modifier value or the default value if not found</returns>
        /// <exception cref="ArgumentException">Thrown when statName is null, empty, or whitespace</exception>
        public int GetStatModifier(string statName, int defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(statName))
                throw new ArgumentException("Stat name cannot be null, empty, or whitespace", nameof(statName));
            
            return StatModifiers.TryGetValue(statName, out var modifier) ? modifier : defaultValue;
        }

        /// <summary>
        /// Updates the core details of the item template
        /// </summary>
        /// <param name="name">The new name for the template</param>
        /// <param name="description">The new description for the template</param>
        /// <param name="itemType">The new item type</param>
        /// <param name="value">The new base value</param>
        /// <param name="isConsumable">Whether the item can be consumed</param>
        /// <param name="isEquippable">Whether the item can be equipped</param>
        /// <param name="equipmentSlot">The equipment slot for equippable items</param>
        /// <exception cref="ArgumentException">Thrown when name or description is null, empty, or whitespace</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is negative</exception>
        public void UpdateDetails(string name, string description, ItemType itemType, int value, bool isConsumable, bool isEquippable, EquipmentSlot? equipmentSlot)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null, empty, or whitespace", nameof(name));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be null, empty, or whitespace", nameof(description));
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be negative");

            Name = name;
            Description = description;
            ItemType = itemType;
            Value = value;
            IsConsumable = isConsumable;
            IsEquippable = isEquippable;
            EquipmentSlot = equipmentSlot;
        }

        /// <summary>
        /// Updates only the basic properties of the item template
        /// </summary>
        /// <param name="name">The new name for the template</param>
        /// <param name="description">The new description for the template</param>
        /// <param name="value">The new base value</param>
        /// <exception cref="ArgumentException">Thrown when name or description is null, empty, or whitespace</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is negative</exception>
        public void UpdateBasicDetails(string name, string description, int value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null, empty, or whitespace", nameof(name));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be null, empty, or whitespace", nameof(description));
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be negative");

            Name = name;
            Description = description;
            Value = value;
        }

        /// <summary>
        /// Clears all stat modifiers from the item template
        /// </summary>
        /// <returns>True if any modifiers were cleared, false if there were no modifiers</returns>
        public bool ClearStatModifiers()
        {
            if (StatModifiers.Count == 0)
            {
                return false; // No modifiers to clear
            }
            StatModifiers.Clear();
            return true; // Modifiers were cleared
        }

        /// <summary>
        /// Checks if the item template has a specific stat modifier
        /// </summary>
        /// <param name="statName">The name of the stat modifier to check</param>
        /// <returns>True if the modifier exists, false otherwise</returns>
        /// <exception cref="ArgumentException">Thrown when statName is null, empty, or whitespace</exception>
        public bool HasStatModifier(string statName)
        {
            if (string.IsNullOrWhiteSpace(statName))
                throw new ArgumentException("Stat name cannot be null, empty, or whitespace", nameof(statName));
            
            return StatModifiers.ContainsKey(statName);
        }

        /// <summary>
        /// Gets the total count of stat modifiers
        /// </summary>
        /// <returns>The number of stat modifiers</returns>
        public int GetStatModifierCount()
        {
            return StatModifiers.Count;
        }

        /// <summary>
        /// Updates the value of the item template
        /// </summary>
        /// <param name="newValue">The new value for the item</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is negative</exception>
        public void UpdateValue(int newValue)
        {
            if (newValue < 0)
                throw new ArgumentOutOfRangeException(nameof(newValue), "Value cannot be negative");
            
            Value = newValue;
        }
    }
}