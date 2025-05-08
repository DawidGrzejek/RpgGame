using RpgGame.Domain.Entities.Inventory;
using RpgGame.Domain.Enums;
using RpgGame.Domain.Events.Characters;
using RpgGame.Domain.Interfaces.Characters;
using RpgGame.Domain.Interfaces.Inventory;
using RpgGame.Domain.Interfaces.Items;
using System;
using System.Collections.Generic;

namespace RpgGame.Domain.Entities.Characters.Player
{
    public class Warrior : Base.PlayerCharacter
    {
        // Warrior-specific properties
        private const int _baseHealth = 150;
        private const int _baseStrength = 20;
        private const int _baseDefense = 10;

        // Warrior-specific inventory defaults
        private const int _defaultInventoryCapacity = 20; // Warriors can carry more items
        private const int _defaultStartingGold = 50;

        /// <summary>
        /// Constructor for the Warrior class
        /// </summary>
        private Warrior(string name, IInventory inventory)
            : base(name, _baseHealth, _baseStrength, _baseDefense, inventory)
        {
            // Warrior-specific initialization can go here
        }

        /// <summary>
        /// Private constructor for event sourcing
        /// </summary>
        private Warrior(bool forEventSourcing) : base(forEventSourcing)
        {
            // Will be populated by Apply methods
        }

        /// <summary>
        /// Factory method to create a Warrior instance with a default inventory
        /// </summary>
        /// <param name="name">The character's name</param>
        /// <returns>A new Warrior instance</returns>
        public static Warrior Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            // Create default warrior inventory
            var defaultInventory = CreateDefaultInventory();

            // Create the warrior
            var warrior = new Warrior(name, defaultInventory);

            // Raise domain event
            warrior.RaiseDomainEvent((id, version) => new CharacterCreatedEvent(
                id,
                version,
                name,
                CharacterType.Warrior
            ));

            return warrior;
        }

        /// <summary>
        /// Factory method to create a Warrior instance with a custom inventory
        /// </summary>
        /// <param name="name">The character's name</param>
        /// <param name="inventory">The custom inventory to use</param>
        /// <returns>A new Warrior instance</returns>
        public static Warrior Create(string name, IInventory inventory)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            if (inventory == null)
            {
                throw new ArgumentNullException(nameof(inventory), "Inventory cannot be null.");
            }

            return new Warrior(name, inventory);
        }

        /// <summary>
        /// internal factory method for event sourcing
        /// </summary>
        /// <returns></returns>
        internal static Warrior CreateForEventSourcing()
        {
            return new Warrior(true);
        }

        /// <summary>
        /// Creates the default inventory for a new Warrior
        /// </summary>
        /// <returns>A new Inventory instance with Warrior defaults</returns>
        private static IInventory CreateDefaultInventory()
        {
            // In a full implementation, we would add starting items here
            // For example: a basic sword, some health potions, etc.
            // This would require the implementation of these item classes
            List<IItem> startingItems = new List<IItem>();

            // Add warrior starting items when they are implemented
            // startingItems.Add(new Sword("Rusty Sword", 5));
            // startingItems.Add(new HealthPotion(20));

            return RpgGame.Domain.Entities.Inventory.Inventory.Create(_defaultInventoryCapacity, _defaultStartingGold, startingItems);
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

            // Example special ability: Powerful attack that deals double damage
            int damage = CalculateDamage() * 2;
            target.TakeDamage(damage);

            // Optionally, trigger any special ability-related events
            OnBeforeAttack(target, damage);
            OnAfterAttack(target, damage);
        }
    }
}