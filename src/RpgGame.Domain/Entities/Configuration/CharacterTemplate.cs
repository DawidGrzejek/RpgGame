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
        public string Name { get; private set; }
        public string Description { get; private set; }
        public CharacterType CharacterType { get; private set; }
        public NPCBehavior? NPCBehavior { get; private set; }
        public PlayerClass? PlayerClass { get; private set; }
        
        // Base stats that will be applied to characters
        public CharacterStats BaseStats { get; private set; }
        
        // Configuration data (replaces hard-coded special abilities)
        public Dictionary<string, object> ConfigurationData { get; private set; }
        
        // References to ability templates
        public List<Guid> AbilityIds { get; private set; }
        
        // Loot table for enemies
        public List<Guid> LootTableIds { get; private set; }
        
        // AI behavior data for NPCs
        public Dictionary<string, object> BehaviorData { get; private set; }

        // Private constructor for EF Core
        private CharacterTemplate() 
        {
            ConfigurationData = new Dictionary<string, object>();
            AbilityIds = new List<Guid>();
            LootTableIds = new List<Guid>();
            BehaviorData = new Dictionary<string, object>();
        }

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

        // Configuration methods
        public void AddConfiguration(string key, object value)
        {
            ConfigurationData[key] = value;
        }

        public void AddAbility(Guid abilityId)
        {
            if (!AbilityIds.Contains(abilityId))
            {
                AbilityIds.Add(abilityId);
            }
        }

        public void AddLootItem(Guid itemId)
        {
            if (!LootTableIds.Contains(itemId))
            {
                LootTableIds.Add(itemId);
            }
        }

        public void SetBehaviorData(string key, object value)
        {
            BehaviorData[key] = value;
        }

        public T GetConfiguration<T>(string key, T defaultValue = default)
        {
            if (ConfigurationData.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return defaultValue;
        }

        public void UpdateBaseStats(CharacterStats newStats)
        {
            BaseStats = newStats ?? throw new ArgumentNullException(nameof(newStats));
        }
    }
}