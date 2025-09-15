using System;
using System.Collections.Generic;
using RpgGame.Domain.Base;
using RpgGame.Domain.Enums;
using RpgGame.Domain.ValueObjects;

namespace RpgGame.Domain.Entities.Configuration
{
    /// <summary>
    /// Base template for all character types - stored in database
    /// This replaces the need for multiple inheritance hierarchies
    /// </summary>
    public class CharacterTemplate : DomainEntity
    {
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public CharacterType CharacterType { get; protected set; }
        public NPCBehavior? NPCBehavior { get; protected set; }
        public PlayerClass? PlayerClass { get; protected set; }

        // Base stats that will be applied to characters
        public CharacterStats BaseStats { get; protected set; }

        // Configuration data (replaces hard-coded special abilities)
        public Dictionary<string, object> ConfigurationData { get; protected set; }

        // References to ability templates
        public List<Guid> AbilityIds { get; protected set; }

        // Loot table for enemies
        public List<Guid> LootTableIds { get; protected set; }

        // AI behavior data for NPCs
        public Dictionary<string, object> BehaviorData { get; protected set; }

        // Private constructor for EF Core
        private CharacterTemplate()
        {
            ConfigurationData = new Dictionary<string, object>();
            AbilityIds = new List<Guid>();
            LootTableIds = new List<Guid>();
            BehaviorData = new Dictionary<string, object>();
        }

        /// <summary>
        /// Creates a new character template
        /// </summary>
        /// <param name="name">Name of the character</param>
        /// <param name="description">Description of the character</param>
        /// <param name="characterType">Type of the character</param>
        /// <param name="baseStats">Base stats of the character</param>
        /// <param name="npcBehavior">Optional NPC behavior for non-player characters</param>
        /// <param name="playerClass">Optional player class for player characters</param>
        /// <exception cref="ArgumentNullException">Thrown if name, description, or baseStats are null</exception>
        /// <exception cref="ArgumentException">Thrown if characterType is not valid</exception>
        /// <returns>A new instance of CharacterTemplate</returns>
        public CharacterTemplate(
            string name,
            string description,
            CharacterType characterType,
            CharacterStats baseStats,
            NPCBehavior? npcBehavior = null,
            PlayerClass? playerClass = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            CharacterType = characterType;
            BaseStats = baseStats ?? throw new ArgumentNullException(nameof(baseStats));
            NPCBehavior = npcBehavior;
            PlayerClass = playerClass;

            ConfigurationData = new Dictionary<string, object>();
            AbilityIds = new List<Guid>();
            LootTableIds = new List<Guid>();
            BehaviorData = new Dictionary<string, object>();
        }

        // 🎯 Template-Driven Design: Type-Safe Factory Methods

        /// <summary>
        /// Creates a player character template with specified class
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when name or description is null, empty, or whitespace</exception>
        /// <exception cref="ArgumentNullException">Thrown when baseStats or abilityIds is null</exception>
        public static CharacterTemplate CreatePlayerTemplate(
            string name,
            string description,
            CharacterStats baseStats,
            List<Guid> abilityIds,
            PlayerClass playerClass = Enums.PlayerClass.Warrior)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null, empty, or whitespace", nameof(name));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be null, empty, or whitespace", nameof(description));
            if (baseStats == null)
                throw new ArgumentNullException(nameof(baseStats));
            if (abilityIds == null)
                throw new ArgumentNullException(nameof(abilityIds));

            var template = new CharacterTemplate(name, description, CharacterType.Player, baseStats, null, playerClass);
            template.AbilityIds.AddRange(abilityIds);

            return template;
        }

        /// <summary>
        /// Creates an NPC template with specified behavior
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when name or description is null, empty, or whitespace</exception>
        /// <exception cref="ArgumentNullException">Thrown when baseStats or abilityIds is null</exception>
        public static CharacterTemplate CreateNPCTemplate(
            string name,
            string description,
            CharacterStats baseStats,
            List<Guid> abilityIds,
            NPCBehavior npcBehavior = Enums.NPCBehavior.Passive)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null, empty, or whitespace", nameof(name));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be null, empty, or whitespace", nameof(description));
            if (baseStats == null)
                throw new ArgumentNullException(nameof(baseStats));
            if (abilityIds == null)
                throw new ArgumentNullException(nameof(abilityIds));

            var template = new CharacterTemplate(name, description, CharacterType.NPC, baseStats, npcBehavior, null);
            template.AbilityIds.AddRange(abilityIds);

            return template;
        }

        /// <summary>
        /// Creates an enemy template with combat configuration
        /// </summary>
        public static CharacterTemplate CreateEnemyTemplate(
            string name,
            string description,
            CharacterStats baseStats,
            List<Guid>? abilityIds = null,
            List<Guid>? lootTableIds = null,
            int experienceReward = 0)
        {
            var template = new CharacterTemplate(name, description, CharacterType.NPC, baseStats, Enums.NPCBehavior.Aggressive, null);

            if (abilityIds != null)
            {
                template.AbilityIds.AddRange(abilityIds);
            }

            if (lootTableIds != null)
            {
                template.LootTableIds.AddRange(lootTableIds);
            }

            // Enemy-specific configuration
            template.AddConfiguration("ExperienceReward", experienceReward);
            template.AddConfiguration("IsEnemy", true);

            return template;
        }

        

        /// <summary>
        /// Adds a configuration key-value pair to the character template
        /// </summary>
        /// <param name="key">The configuration key</param>
        /// <param name="value">The configuration value</param>
        /// <exception cref="ArgumentException">Thrown when key is null, empty, or whitespace</exception>
        /// <exception cref="InvalidOperationException">Thrown when key already exists</exception>
        public void AddConfiguration(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null, empty, or whitespace", nameof(key));
            
            if (ConfigurationData.ContainsKey(key))
                throw new InvalidOperationException($"Configuration key '{key}' already exists");
            
            ConfigurationData[key] = value;
        }

        /// <summary>
        /// Gets a configuration value by key, with an optional default value
        /// </summary>
        /// <param name="key">The configuration key</param>
        /// <param name="defaultValue">The default value to return if the key is not found</param>
        /// <returns>The configuration value, or the default value if not found</returns>
        public T GetConfiguration<T>(string key, T defaultValue = default)
        {
            if (ConfigurationData.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return defaultValue;
        }
        
        /// <summary>
        /// Removes a configuration key-value pair from the character template
        /// </summary>
        /// <param name="key">The configuration key</param>
        /// <returns>True if the key was found and removed, false otherwise</returns>
        public bool RemoveConfiguration(string key)
        {
            return ConfigurationData.Remove(key);
        }

        /// <summary>
        /// Clears all configuration data from the character template
        /// </summary>
        public bool ClearConfiguration()
        {
            if (ConfigurationData.Count == 0)
            {
                return false; // No configuration to clear
            }
            ConfigurationData.Clear();
            return true; // Configuration cleared successfully
        }

        /// <summary>
        /// Adds a special ability to the character template
        /// </summary>
        /// <param name="abilityId">The ID of the ability to add</param>
        /// <exception cref="ArgumentException">Thrown when abilityId is empty</exception>
        /// <exception cref="InvalidOperationException">Thrown when ability already exists</exception>
        public void AddAbility(Guid abilityId)
        {
            if (abilityId == Guid.Empty)
                throw new ArgumentException("Ability ID cannot be empty", nameof(abilityId));
            
            if (AbilityIds.Contains(abilityId))
                throw new InvalidOperationException($"Ability {abilityId} already exists in template");
            
            AbilityIds.Add(abilityId);
        }

        /// <summary>
        /// Removes a special ability from the character template
        /// </summary>
        /// <param name="abilityId">The ID of the ability to remove</param>
        public bool RemoveAbility(Guid abilityId)
        {
            return AbilityIds.Remove(abilityId);
        }

        /// <summary>
        /// Adds a loot item to the character's loot table
        /// </summary>
        /// <param name="itemId">The ID of the loot item to add</param>
        public void AddLootItem(Guid itemId)
        {
            if (!LootTableIds.Contains(itemId))
            {
                LootTableIds.Add(itemId);
            }
        }

        /// <summary>
        /// Removes a loot item from the character's loot table
        /// </summary>
        /// <param name="itemId">The ID of the loot item to remove</param>
        public bool RemoveLootItem(Guid itemId)
        {
            return LootTableIds.Remove(itemId);
        }

        /// <summary>
        /// Sets a behavior data key-value pair for NPCs
        /// </summary>
        /// <param name="key">The behavior data key</param>
        /// <param name="value">The behavior data value</param>
        public void SetBehaviorData(string key, object value)
        {
            BehaviorData[key] = value;
        }

        /// <summary>
        /// Gets a behavior data value by key, with an optional default value
        /// </summary>
        /// <param name="key">The behavior data key</param>
        /// <param name="defaultValue">The default value to return if the key is not found</param>
        /// <returns>The behavior data value, or the default value if not found</returns>
        public T GetBehaviorData<T>(string key, T defaultValue = default)
        {
            if (BehaviorData.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return defaultValue;
        }

        /// <summary>
        /// Removes a behavior data key-value pair from the character template
        /// </summary>
        /// <param name="key">The behavior data key</param>
        /// <returns>True if the key was found and removed, false otherwise</returns>
        public bool RemoveBehaviorData(string key)
        {
            return BehaviorData.Remove(key);
        }

        /// <summary>
        /// Updates the core details of the character template
        /// </summary>
        /// <param name="name">The new name for the template</param>
        /// <param name="description">The new description for the template</param>
        /// <param name="baseStats">The new base stats for the template</param>
        /// <exception cref="ArgumentException">Thrown when name is null, empty, or whitespace</exception>
        /// <exception cref="ArgumentNullException">Thrown when baseStats is null</exception>
        public void UpdateDetails(string name, string description, CharacterStats baseStats)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null, empty, or whitespace", nameof(name));
            
            Name = name;
            Description = description ?? throw new ArgumentNullException(nameof(description));
            BaseStats = baseStats ?? throw new ArgumentNullException(nameof(baseStats));
        }

        /// <summary>
        /// Updates the base stats of the character template
        /// </summary>
        /// <param name="newStats">The new base stats to apply</param>
        public void UpdateBaseStats(CharacterStats newStats)
        {
            BaseStats = newStats ?? throw new ArgumentNullException(nameof(newStats));
        }

        /// <summary>
        /// Clears all abilities from the character template
        /// </summary>
        public void ClearAbilities()
        {
            AbilityIds.Clear();
        }
    }
}