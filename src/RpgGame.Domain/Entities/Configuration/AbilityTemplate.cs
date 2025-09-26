using System;
using System.Collections.Generic;
using RpgGame.Domain.Base;
using RpgGame.Domain.Enums;

namespace RpgGame.Domain.Entities.Configuration
{
    /// <summary>
    /// Template for abilities - completely data-driven approach
    /// Replaces hard-coded switch statements with database configuration
    /// </summary>
    public class AbilityTemplate : DomainEntity
    {
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public AbilityType AbilityType { get; protected set; }
        public int ManaCost { get; protected set; }
        public int Cooldown { get; protected set; }
        
        // Flexible effect system - no more hard-coded abilities!
        public List<AbilityEffect> Effects { get; protected set; }
        
        // Targeting information
        public TargetType TargetType { get; protected set; }
        public int Range { get; protected set; }
        
        // Visual/Audio configuration
        public string AnimationName { get; protected set; }
        public string SoundEffect { get; protected set; }
        
        // Requirements to use this ability
        public Dictionary<string, object> Requirements { get; protected set; }

        private AbilityTemplate() 
        {
            Effects = new List<AbilityEffect>();
            Requirements = new Dictionary<string, object>();
        }

        public AbilityTemplate(
            string name,
            string description,
            AbilityType abilityType,
            TargetType targetType,
            int manaCost = 0,
            int cooldown = 0,
            int range = 1)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            AbilityType = abilityType;
            TargetType = targetType;
            ManaCost = Math.Max(0, manaCost);
            Cooldown = Math.Max(0, cooldown);
            Range = Math.Max(1, range);
            
            Effects = new List<AbilityEffect>();
            Requirements = new Dictionary<string, object>();
            AnimationName = "";
            SoundEffect = "";
        }

        public void AddEffect(AbilityEffect effect)
        {
            Effects.Add(effect ?? throw new ArgumentNullException(nameof(effect)));
        }

        public void SetVisuals(string animationName, string soundEffect)
        {
            AnimationName = animationName ?? "";
            SoundEffect = soundEffect ?? "";
        }

        public void AddRequirement(string key, object value)
        {
            Requirements[key] = value;
        }

        public void UpdateDetails(string name, string description, AbilityType abilityType, TargetType targetType, int manaCost = 0, int cooldown = 0, int range = 1)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            AbilityType = abilityType;
            TargetType = targetType;
            ManaCost = Math.Max(0, manaCost);
            Cooldown = Math.Max(0, cooldown);
            Range = Math.Max(1, range);
        }

        public bool MeetsRequirements(Dictionary<string, object> characterData)
        {
            foreach (var requirement in Requirements)
            {
                if (!characterData.TryGetValue(requirement.Key, out var value) || 
                    !value.Equals(requirement.Value))
                {
                    return false;
                }
            }
            return true;
        }
    }

    /// <summary>
    /// Individual effect within an ability - fully data-driven
    /// </summary>
    public class AbilityEffect
    {
        public EffectType EffectType { get; set; }
        public int BasePower { get; set; }
        public int Duration { get; set; } // 0 = instant
        public Dictionary<string, object> Parameters { get; set; }

        public AbilityEffect()
        {
            Parameters = new Dictionary<string, object>();
        }

        public AbilityEffect(EffectType effectType, int basePower, int duration = 0)
        {
            EffectType = effectType;
            BasePower = basePower;
            Duration = Math.Max(0, duration);
            Parameters = new Dictionary<string, object>();
        }

        public void SetParameter(string key, object value)
        {
            Parameters[key] = value;
        }

        public T GetParameter<T>(string key, T defaultValue = default)
        {
            if (Parameters.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return defaultValue;
        }
    }
}