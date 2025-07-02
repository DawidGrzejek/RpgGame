using System;
using System.Collections.Generic;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Interfaces.Characters;
using RpgGame.Domain.Interfaces.Items;

namespace RpgGame.Domain.Entities.Characters.NPC.Enemy
{
    /// <summary>
    /// Base class for all enemy characters
    /// </summary>
    public abstract class EnemyBase : NonPlayerCharacter
    {
        // Additional fields for enemies
        protected int _experienceReward;
        protected List<IItem> _lootTable;

        // Properties
        public int ExperienceReward => _experienceReward;
        public IReadOnlyList<IItem> LootTable => _lootTable.AsReadOnly();

        /// <summary>
        /// Constructor for enemy characters
        /// </summary>
        protected EnemyBase(
            string name,
            int health,
            int strength,
            int defense,
            int experienceReward)
            : base(name, health, strength, defense, false, $"{name} growls menacingly!")
        {
            if (experienceReward < 0)
                throw new ArgumentException("Experience reward cannot be negative", nameof(experienceReward));

            _experienceReward = experienceReward;
            _lootTable = new List<IItem>();
        }

        /// <summary>
        /// Drops a random item from the loot table
        /// </summary>
        public IItem DropLoot()
        {
            if (_lootTable.Count == 0)
                return null;

            Random rnd = new Random();
            IItem droppedItem = _lootTable[rnd.Next(_lootTable.Count)];

            Console.WriteLine($"{Name} dropped {droppedItem.Name}!");
            return droppedItem;
        }

        /// <summary>
        /// Overrides the interaction behavior to initiate combat
        /// </summary>
        public override void Interact(IPlayerCharacter player)
        {
            // Enemies attack when interacted with
            Console.WriteLine($"{Name} attacks you!");
            Attack(player);
        }

        /// <summary>
        /// Called when the enemy is defeated
        /// </summary>
        protected override void OnDeath()
        {
            base.OnDeath();
            // The experience reward is handled by the combat system
        }
    }
}