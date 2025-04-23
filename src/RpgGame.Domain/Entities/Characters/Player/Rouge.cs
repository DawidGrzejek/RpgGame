using RpgGame.Domain.Entities.Inventory;
using RpgGame.Domain.Interfaces.Characters;
using RpgGame.Domain.Interfaces.Inventory;
using RpgGame.Domain.Interfaces.Items;
using System;
using System.Collections.Generic;

namespace RpgGame.Domain.Entities.Characters.Player
{
    public class Rogue : Base.PlayerCharacter
    {
        // Rogue-specific properties
        private const int _baseHealth = 120;
        private const int _baseStrength = 15;
        private const int _baseDefense = 8;
        private const double _baseCriticalChance = 0.15; // 15% base critical hit chance

        // Rogue-specific inventory defaults
        private const int _defaultInventoryCapacity = 25; // Rogues can carry more items for looting
        private const int _defaultStartingGold = 100; // Rogues start with more gold

        // Rogue-specific stat
        private double _criticalChance;

        // Properties
        public double CriticalChance => _criticalChance;

        /// <summary>
        /// Constructor for the Rogue class
        /// </summary>
        private Rogue(string name, IInventory inventory)
            : base(name, _baseHealth, _baseStrength, _baseDefense, inventory)
        {
            // Rogue-specific initialization
            _criticalChance = _baseCriticalChance;
        }

        /// <summary>
        /// Factory method to create a Rogue instance with a default inventory
        /// </summary>
        /// <param name="name">The character's name</param>
        /// <returns>A new Rogue instance</returns>
        public static Rogue Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            // Create default rogue inventory
            var defaultInventory = CreateDefaultInventory();

            return new Rogue(name, defaultInventory);
        }

        /// <summary>
        /// Factory method to create a Rogue instance with a custom inventory
        /// </summary>
        /// <param name="name">The character's name</param>
        /// <param name="inventory">The custom inventory to use</param>
        /// <returns>A new Rogue instance</returns>
        public static Rogue Create(string name, IInventory inventory)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            if (inventory == null)
            {
                throw new ArgumentNullException(nameof(inventory), "Inventory cannot be null.");
            }

            return new Rogue(name, inventory);
        }

        /// <summary>
        /// Creates the default inventory for a new Rogue
        /// </summary>
        /// <returns>A new Inventory instance with Rogue defaults</returns>
        private static IInventory CreateDefaultInventory()
        {
            // In a full implementation, we would add starting items here
            // For example: daggers, lockpicks, etc.
            List<IItem> startingItems = new List<IItem>();

            // Add rogue starting items when they are implemented
            // startingItems.Add(new Dagger("Sharp Dagger", 4));
            // startingItems.Add(new Lockpicks(5));

            return RpgGame.Domain.Entities.Inventory.Inventory.Create(_defaultInventoryCapacity, _defaultStartingGold, startingItems);
        }

        /// <summary>
        /// Override the attack method to include critical hits
        /// </summary>
        public override void Attack(ICharacter target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target), "Attack target cannot be null");

            if (!IsAlive)
            {
                Console.WriteLine($"{Name} cannot attack while defeated");
                return;
            }

            // Check for critical hit
            Random rnd = new Random();
            bool isCritical = rnd.NextDouble() < _criticalChance;

            // Calculate damage
            int damage = CalculateDamage();

            // Apply critical multiplier if critical hit
            if (isCritical)
            {
                damage = (int)(damage * 2.0); // Double damage on critical
                Console.WriteLine($"{Name} scores a CRITICAL HIT on {target.Name}!");
            }

            // Apply damage
            OnBeforeAttack(target, damage);
            target.TakeDamage(damage);
            OnAfterAttack(target, damage);
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

            // Example special ability: Backstab (guaranteed critical hit with bonus damage)
            int damage = CalculateDamage() * 3; // Triple damage for backstab

            Console.WriteLine($"{Name} performs a deadly backstab on {target.Name}!");
            target.TakeDamage(damage);
        }

        /// <summary>
        /// Override of the level up method to increase critical chance as well
        /// </summary>
        public override void LevelUp()
        {
            base.LevelUp();

            // Increase critical hit chance on level up (small increment)
            _criticalChance += 0.01; // +1% per level

            Console.WriteLine($"{Name}'s critical hit chance increased to {_criticalChance:P0}!");
        }
    }
}