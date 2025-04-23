using System;
using System.Collections.Generic;
using RpgGame.Domain.Enums;
using RpgGame.Domain.Interfaces.Characters;
using RpgGame.Domain.Interfaces.Inventory;
using RpgGame.Domain.Interfaces.Items;

namespace RpgGame.Domain.Entities.Characters.Base
{
    /// <summary>
    /// Base class for all player characters
    /// </summary>
    public abstract class PlayerCharacter : Character, IPlayerCharacter
    {
        // Additional encapsulated fields for player characters
        protected int _experience;
        protected int _baseExperienceToNextLevel;
        protected IInventory _inventory;
        protected Dictionary<EquipmentSlot, IEquipment> _equippedItems;

        // Properties
        public int Experience => _experience;
        public int ExperienceToNextLevel => _baseExperienceToNextLevel * Level;
        public IInventory Inventory => _inventory;

        /// <summary>
        /// Constructor for player characters
        /// </summary>
        protected PlayerCharacter(
            string name,
            int health,
            int strength,
            int defense,
            IInventory inventory)
            : base(name, health, strength, defense)
        {
            _experience = 0;
            _baseExperienceToNextLevel = 100;
            _inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
            _equippedItems = new Dictionary<EquipmentSlot, IEquipment>();
        }

        /// <summary>
        /// Adds experience points to the character and levels up if threshold is reached
        /// </summary>
        public void GainExperience(int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Experience amount cannot be negative", nameof(amount));

            _experience += amount;

            OnExperienceGained(amount);

            // Check if player can level up
            while (_experience >= ExperienceToNextLevel)
            {
                _experience -= ExperienceToNextLevel;
                LevelUp();
            }
        }

        /// <summary>
        /// Equips an item to the character
        /// </summary>
        public void EquipItem(IEquipment item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (!_inventory.Items.Contains(item))
            {
                Console.WriteLine($"Cannot equip {item.Name}: Item not in inventory");
                return;
            }

            // Check if we already have something equipped in this slot
            if (_equippedItems.TryGetValue(item.Slot, out IEquipment currentItem))
            {
                // Unequip current item
                currentItem.OnUnequip(this);
                _inventory.AddItem(currentItem);
                _equippedItems.Remove(item.Slot);
            }

            // Remove item from inventory and equip it
           _inventory.RemoveItem(item);
           _equippedItems[item.Slot] = item;
            item.OnEquip(this);

            OnItemEquipped(item);
        }

        /// <summary>
        /// Uses a consumable item from inventory
        /// </summary>
        public void UseItem(IItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (!_inventory.Items.Contains(item))
            {
                Console.WriteLine($"Cannot use {item.Name}: Item not in inventory");
                return;
            }
            
            if (item is IConsumable consumable)
            {
                _inventory.RemoveItem(item);
                consumable.OnUse(this);
                OnItemUsed(item);
            }
            else
            {
                Console.WriteLine($"Cannot use {item.Name}. It is not a consumable item.");
            }
        }

        /// <summary>
        /// Uses the character's special ability
        /// </summary>
        public abstract void UseSpecialAbility(ICharacter target);

        // Protected event methods for derived classes

        protected virtual void OnExperienceGained(int amount)
        {
            Console.WriteLine($"{Name} gained {amount} experience points. Total: {_experience}/{ExperienceToNextLevel}");
        }

        protected virtual void OnItemEquipped(IEquipment item)
        {
            Console.WriteLine($"{Name} equipped {item.Name}");
        }

        protected virtual void OnItemUsed(IItem item)
        {
            Console.WriteLine($"{Name} used {item.Name}");
        }

        // Override base character methods

        protected override void OnDeath()
        {
            base.OnDeath();
            Console.WriteLine("Game Over! Your character has been defeated.");
        }
    }
}