using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RpgGame.Domain.Base;
using RpgGame.Domain.Enums;

namespace RpgGame.Domain.Entities.Configuration
{
    public class EnemyTemplate : DomainEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int BaseHealth { get; private set; }
        public int BaseStrength { get; private set; }
        public int BaseDefense { get; private set; }
        public int ExperienceReward { get; private set; }
        public EnemyType EnemyType { get; private set; }
        public List<string> PossibleLoot { get; private set; }
        public string AttackPattern { get; private set; }
        public Dictionary<string, object> SpecialAbilities { get; private set; }

        public EnemyTemplate(
            string name,
            string description,
            int baseHealth,
            int baseStrength,
            int baseDefense,
            int experienceReward,
            EnemyType enemyType,
            List<string>? possibleLoot = null,
            string? attackPattern = null,
            Dictionary<string, object>? specialAbilities = null)
        {
            Name = name;
            Description = description;
            BaseHealth = baseHealth;
            BaseStrength = baseStrength;
            BaseDefense = baseDefense;
            ExperienceReward = experienceReward;
            EnemyType = enemyType;
            PossibleLoot = possibleLoot ?? new List<string>();
            AttackPattern = attackPattern ?? string.Empty;
            SpecialAbilities = specialAbilities ?? new Dictionary<string, object>();
        }

        public void AddLootItem(string lootItem)
        {
            PossibleLoot.Add(lootItem);
        }

        public void RemoveLootItem(string lootItem)
        {
            PossibleLoot.Remove(lootItem);
        }

        public void AddSpecialAbility(string abilityName, object abilityData)
        {
            if (SpecialAbilities.ContainsKey(abilityName))
            {
                throw new InvalidOperationException($"Ability '{abilityName}' already exists.");
            }
            SpecialAbilities[abilityName] = abilityData;
        }

        public void UpdateDetails(string name, string description, int baseHealth, int baseStrength, int baseDefense, int experienceReward, EnemyType enemyType)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            BaseHealth = baseHealth;
            BaseStrength = baseStrength;
            BaseDefense = baseDefense;
            ExperienceReward = experienceReward;
            EnemyType = enemyType;
        }

        public void ClearLoot()
        {
            PossibleLoot.Clear();
        }

        public void ClearSpecialAbilities()
        {
            SpecialAbilities.Clear();
        }
    }
}