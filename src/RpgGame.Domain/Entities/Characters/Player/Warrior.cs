using RpgGame.Domain.Interfaces.Characters;
using RpgGame.Domain.Interfaces.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Domain.Entities.Characters.Player
{
    public class Warrior : Base.PlayerCharacter
    {
        // Warrior-specific properties
        private const int _baseHealth = 150;
        private const int _baseStrength = 20;
        private const int _baseDefense = 10;

        /// <summary>
        /// Constructor for the Warrior class
        /// </summary>
        private Warrior(string name, IInventory inventory)
            : base(name, _baseHealth, _baseStrength, _baseDefense, inventory)
        {
            // Warrior-specific initialization can go here
        }

        /// Factory method to create a Warrior instance
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
