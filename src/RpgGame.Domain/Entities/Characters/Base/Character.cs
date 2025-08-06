using System;
using System.Collections.Generic;
using System.Linq;
using RpgGame.Domain.Base;
using RpgGame.Domain.Enums;
using RpgGame.Domain.ValueObjects;
using RpgGame.Domain.Entities.Configuration;
using RpgGame.Domain.Events.Characters;
using RpgGame.Domain.Interfaces.Characters;

namespace RpgGame.Domain.Entities.Characters.Base
{
    /// <summary>
    /// Single character entity that uses composition instead of inheritance
    /// This replaces the complex inheritance hierarchy with template-driven design
    /// </summary>
    public class Character : DomainEntity, ICharacter, IPlayerCharacter
    {
        // Core character properties
        public string Name { get; private set; }
        public CharacterType Type { get; private set; } // Player or NPC
        public CharacterStats Stats { get; private set; } // Record type for immutable stats
        public bool IsAlive => Stats.CurrentHealth > 0;
        
        // Template reference for configuration
        public Guid? TemplateId { get; private set; }
        
        // Character-specific data
        public Dictionary<string, object> CustomData { get; private set; }
        public List<Guid> Abilities { get; private set; }
        public List<Guid> InventoryIds { get; private set; } // Item IDs for template system
        
        // Inventory system for IPlayerCharacter interface
        private Entities.Inventory.Inventory _inventory;
        public Interfaces.Inventory.IInventory Inventory => _inventory ??= Entities.Inventory.Inventory.Create(50); // Default 50 capacity

        // For player characters
        public PlayerClass? PlayerClass { get; private set; } 
        public int Experience { get; private set; }
        public int ExperienceToNextLevel => (Stats.Level * 1000) - Experience; // Level * 1000 is requirement for next level
        
        // For NPCs
        public NPCBehavior? NPCBehavior { get; private set; }

        // Compatibility properties for existing interfaces
        public int Health => Stats.CurrentHealth;
        public int MaxHealth => Stats.MaxHealth;
        public int Level => Stats.Level;
        public int Strength => Stats.Strength;
        public int Defense => Stats.Defense;

        // Private constructor for EF Core
        private Character() 
        {
            CustomData = new Dictionary<string, object>();
            Abilities = new List<Guid>();
            InventoryIds = new List<Guid>();
        }

        // Factory methods
        public static Character CreatePlayer(
            string name, 
            PlayerClass playerClass,
            CharacterStats baseStats)
        {
            return new Character
            {
                Id = Guid.NewGuid(),
                Name = name,
                Type = CharacterType.Player,
                PlayerClass = playerClass,
                Stats = baseStats,
                CustomData = new Dictionary<string, object>(),
                Abilities = new List<Guid>(),
                InventoryIds = new List<Guid>(),
                Experience = 0
            };
        }

        public static Character CreateNPC(
            string name,
            NPCBehavior behavior,
            CharacterStats stats,
            Guid? templateId = null)
        {
            return new Character
            {
                Id = Guid.NewGuid(),
                Name = name,
                Type = CharacterType.NPC,
                NPCBehavior = behavior,
                Stats = stats,
                TemplateId = templateId,
                CustomData = new Dictionary<string, object>(),
                Abilities = new List<Guid>(),
                InventoryIds = new List<Guid>()
            };
        }

        // Core behaviors - maintaining compatibility with existing interface
        public virtual void Attack(ICharacter target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target), "Attack target cannot be null");

            if (!IsAlive)
            {
                Console.WriteLine($"{Name} cannot attack while defeated");
                return;
            }

            int damage = CalculateDamage();
            Console.WriteLine($"{Name} attacks {target.Name} for {damage} damage!");
            target.TakeDamage(damage);
        }

        public virtual void TakeDamage(int damage)
        {
            if (damage < 0)
                throw new ArgumentException("Damage cannot be negative", nameof(damage));

            var actualDamage = Math.Max(1, damage - Stats.Defense);
            Stats = Stats.WithHealth(Math.Max(0, Stats.CurrentHealth - actualDamage));
            
            Console.WriteLine($"{Name} takes {actualDamage} damage. Health: {Stats.CurrentHealth}/{Stats.MaxHealth}");
            
            if (!IsAlive)
            {
                OnDeath();
            }
        }

        public virtual void Heal(int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Heal amount cannot be negative", nameof(amount));

            if (!IsAlive)
            {
                Console.WriteLine($"{Name} cannot be healed while defeated");
                return;
            }
            
            var newHealth = Math.Min(Stats.MaxHealth, Stats.CurrentHealth + amount);
            var actualHeal = newHealth - Stats.CurrentHealth;
            Stats = Stats.WithHealth(newHealth);
            
            Console.WriteLine($"{Name} heals for {actualHeal} health. Health: {Stats.CurrentHealth}/{Stats.MaxHealth}");
        }

        public virtual void LevelUp()
        {
            int oldLevel = Stats.Level;
            Stats = Stats.LevelUp();
            Experience = 0; // Reset XP for next level
            
            Console.WriteLine($"{Name} leveled up to level {Stats.Level}!");
            Console.WriteLine($"New stats: Health: {Stats.CurrentHealth}/{Stats.MaxHealth}, Strength: {Stats.Strength}, Defense: {Stats.Defense}");

            // Raise domain event
            RaiseDomainEvent((id, version) => new CharacterLeveledUp(
                id,
                version,
                Name,
                oldLevel,
                Stats.Level,
                10, // Health increase from stats
                2,  // Strength increase from stats  
                1   // Defense increase from stats
            ));
        }

        // New methods
        public void AddAbility(Guid abilityId)
        {
            if (!Abilities.Contains(abilityId))
            {
                Abilities.Add(abilityId);
            }
        }
        
        // IPlayerCharacter interface methods
        public void EquipItem(Interfaces.Items.IEquipment item)
        {
            if (Type != CharacterType.Player)
                throw new InvalidOperationException("Only players can equip items");
            
            // TODO: Implement equipment system
            // For now, just add to inventory
            Inventory.AddItem(item);
        }
        
        public void UseItem(Interfaces.Items.IItem item)
        {
            if (Type != CharacterType.Player)
                throw new InvalidOperationException("Only players can use items");
                
            // TODO: Implement item usage system
            // For now, just remove from inventory
            Inventory.RemoveItem(item);
        }
        
        public void UseSpecialAbility(ICharacter target)
        {
            if (Type != CharacterType.Player)
                throw new InvalidOperationException("Only players can use special abilities");
                
            // TODO: Implement special ability system based on PlayerClass
            // For now, just a placeholder
            Console.WriteLine($"{Name} uses a special ability on {target.Name}!");
        }

        public void GainExperience(int xp)
        {
            if (Type != CharacterType.Player) return;
            
            Experience += xp;
            CheckForLevelUp();
        }

        private void CheckForLevelUp()
        {
            var requiredXp = Stats.Level * 1000; // Simple XP calculation
            if (Experience >= requiredXp)
            {
                LevelUp();
            }
        }

        private void OnDeath()
        {
            Console.WriteLine($"{Name} has been defeated!");

            // Raise domain event
            RaiseDomainEvent((id, version) => new CharacterDied(
                id,
                version,
                Name,
                Level,
                "Current Location" // Ideally get this from game context
            ));
        }

        // Template application
        public void ApplyTemplate(CharacterTemplate template)
        {
            TemplateId = template.Id;
            
            // Apply template data
            foreach (var data in template.ConfigurationData)
            {
                CustomData[data.Key] = data.Value;
            }
            
            // Apply template abilities
            foreach (var abilityId in template.AbilityIds)
            {
                AddAbility(abilityId);
            }
        }

        // Compatibility methods
        protected virtual int CalculateDamage()
        {
            Random rnd = new Random();
            return Stats.Strength + rnd.Next(1, 6);
        }

        public virtual CharacterStateExported ExportState()
        {
            return new CharacterStateExported(
                Id,
                Version,
                Name,
                Health,
                MaxHealth,
                Level,
                Strength,
                Defense,
                IsAlive ? 1 : 0,
                GetType().Name
            );
        }

        // Event sourcing method - reconstruct character from events
        public static Character FromEvents(Guid id, System.Collections.Generic.IEnumerable<RpgGame.Domain.Events.Base.IDomainEvent> events)
        {
            if (events == null || !events.Any())
                throw new ArgumentException("Cannot reconstruct character without events");

            Character character = null;

            foreach (var @event in events.OrderBy(e => e.Version))
            {
                switch (@event)
                {
                    case Events.Characters.CharacterCreatedEvent created:
                        // Create the character based on the creation event
                        var baseStats = new CharacterStats(1, 100, 10, 8, 12, 5); // Default stats
                        
                        if (created.CharacterType == CharacterType.Player)
                        {
                            // For players, we need to determine the class from other events or use default
                            character = Character.CreatePlayer(created.Name, PlayerClass.Warrior, baseStats);
                        }
                        else
                        {
                            // For NPCs, use default behavior
                            character = Character.CreateNPC(created.Name, NPCBehavior.Passive, baseStats);
                        }
                        
                        // Set the reconstructed ID and version
                        character.Id = created.AggregateId;
                        character.Version = created.Version;
                        break;

                    case Events.Characters.PlayerGainedExperience expEvent:
                        if (character != null)
                        {
                            character.Experience = expEvent.NewExperience;
                        }
                        break;

                    case Events.Characters.CharacterLeveledUp levelEvent:
                        if (character != null)
                        {
                            character.Stats = character.Stats.LevelUp();
                        }
                        break;

                    case Events.Characters.CharacterDied deathEvent:
                        if (character != null)
                        {
                            character.Stats = character.Stats.WithHealth(0);
                        }
                        break;

                    // Handle other events as needed
                    default:
                        // Ignore unknown events or log them
                        break;
                }
            }

            if (character == null)
                throw new InvalidOperationException("Could not reconstruct character from events - no creation event found");

            return character;
        }
    }
}