using RpgGame.Domain.Entities.Inventory;
using RpgGame.Domain.Interfaces.Characters;
using RpgGame.Domain.Interfaces.Inventory;
using RpgGame.Domain.Interfaces.Items;
using System;
using System.Collections.Generic;

namespace RpgGame.Domain.Entities.Characters.Player
{
    public class Mage : Base.PlayerCharacter
    {
        // Mage-specific properties
        private const int _baseHealth = 100;
        private const int _baseStrength = 10;
        private const int _baseDefense = 5;
        private const int _baseMana = 150;

        // Mage-specific inventory defaults
        private const int _defaultInventoryCapacity = 15; // Mages carry fewer items than warriors
        private const int _defaultStartingGold = 75; // But start with more gold for spell components

        // Mage-specific stat - not in base class
        private int _mana;
        private int _maxMana;

        // Properties for Mage-specific stats
        public int Mana => _mana;
        public int MaxMana => _maxMana;

        /// <summary>
        /// Constructor for the Mage class
        /// </summary>
        private Mage(string name, IInventory inventory)
            : base(name, _baseHealth, _baseStrength, _baseDefense, inventory)
        {
            // Mage-specific initialization
            _maxMana = _baseMana;
            _mana = _maxMana;
        }

        /// <summary>
        /// Factory method to create a Mage instance with a default inventory
        /// </summary>
        /// <param name="name">The character's name</param>
        /// <returns>A new Mage instance</returns>
        public static Mage Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            // Create default mage inventory
            var defaultInventory = CreateDefaultInventory();

            return new Mage(name, defaultInventory);
        }

        /// <summary>
        /// Factory method to create a Mage instance with a custom inventory
        /// </summary>
        /// <param name="name">The character's name</param>
        /// <param name="inventory">The custom inventory to use</param>
        /// <returns>A new Mage instance</returns>
        public static Mage Create(string name, IInventory inventory)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            if (inventory == null)
            {
                throw new ArgumentNullException(nameof(inventory), "Inventory cannot be null.");
            }

            return new Mage(name, inventory);
        }

        /// <summary>
        /// Creates the default inventory for a new Mage
        /// </summary>
        /// <returns>A new Inventory instance with Mage defaults</returns>
        private static IInventory CreateDefaultInventory()
        {
            // In a full implementation, we would add starting items here
            // For example: a staff, mana potions, etc.
            List<IItem> startingItems = new List<IItem>();

            // Add mage starting items when they are implemented
            // startingItems.Add(new Staff("Apprentice Staff", 3));
            // startingItems.Add(new ManaPotion(30));

            return RpgGame.Domain.Entities.Inventory.Inventory.Create(_defaultInventoryCapacity, _defaultStartingGold, startingItems);
        }

        /// <summary>
        /// Method to spend mana for spellcasting
        /// </summary>
        /// <param name="amount">The amount of mana to spend</param>
        /// <returns>True if successful, false if not enough mana</returns>
        public bool SpendMana(int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Mana amount cannot be negative", nameof(amount));

            if (_mana < amount)
            {
                Console.WriteLine($"{Name} doesn't have enough mana. Required: {amount}, Available: {_mana}");
                return false;
            }

            _mana -= amount;
            Console.WriteLine($"{Name} spends {amount} mana. Remaining: {_mana}/{_maxMana}");
            return true;
        }

        /// <summary>
        /// Method to restore mana
        /// </summary>
        /// <param name="amount">The amount of mana to restore</param>
        public void RestoreMana(int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Mana amount cannot be negative", nameof(amount));

            int previousMana = _mana;
            _mana = Math.Min(_maxMana, _mana + amount);
            int actualRestored = _mana - previousMana;

            Console.WriteLine($"{Name} restores {actualRestored} mana. Current: {_mana}/{_maxMana}");
        }

        /// <summary>
        /// Implements the abstract UseSpecialAbility method from PlayerCharacter
        /// </summary>
        /// <param name="target">The target character</param>
        public override void UseSpecialAbility(ICharacter target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target), "Target cannot be null.");
            }

            // Example special ability: Fireball that costs mana
            int manaCost = 20;

            if (!SpendMana(manaCost))
            {
                return; // Not enough mana
            }

            // Calculate damage - mages deal more special ability damage than warriors
            int damage = CalculateDamage() * 3;

            Console.WriteLine($"{Name} casts Fireball at {target.Name}!");
            target.TakeDamage(damage);
        }

        /// <summary>
        /// Override of the level up method to increase mana as well
        /// </summary>
        public override void LevelUp()
        {
            base.LevelUp();

            // Increase mana on level up
            int manaIncrease = 20;
            _maxMana += manaIncrease;
            _mana = _maxMana; // Full mana restore on level up

            Console.WriteLine($"{Name}'s maximum mana increased by {manaIncrease} to {_maxMana}!");
        }
    }
}