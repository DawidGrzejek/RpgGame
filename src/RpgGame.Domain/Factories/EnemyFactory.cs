using System;
using System.Threading.Tasks;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Entities.Configuration;
using RpgGame.Domain.Enums;
using RpgGame.Domain.Interfaces.Repositories;
using RpgGame.Domain.ValueObjects;

namespace RpgGame.Domain.Factories
{
    /// <summary>
    /// Unified factory for all character types using template system
    /// This replaces multiple specialized factories with one template-driven approach
    /// </summary>
    public class CharacterFactory
    {
        private readonly ICharacterTemplateRepository _templateRepository;
        private readonly IAbilityTemplateRepository _abilityRepository;

        public CharacterFactory(
            ICharacterTemplateRepository templateRepository,
            IAbilityTemplateRepository abilityRepository)
        {
            _templateRepository = templateRepository ?? throw new ArgumentNullException(nameof(templateRepository));
            _abilityRepository = abilityRepository ?? throw new ArgumentNullException(nameof(abilityRepository));
        }

        /// <summary>
        /// Creates a character from a template - works for ANY character type
        /// </summary>
        public async Task<Character> CreateFromTemplateAsync(Guid templateId)
        {
            var template = await _templateRepository.GetByIdAsync(templateId);
            if (template == null)
            {
                throw new ArgumentException($"Template with ID {templateId} not found");
            }

            return await CreateFromTemplateAsync(template);
        }

        /// <summary>
        /// Creates a character from a template object
        /// </summary>
        public async Task<Character> CreateFromTemplateAsync(CharacterTemplate template)
        {
            Character character;

            if (template.CharacterType == CharacterType.Player)
            {
                if (!template.PlayerClass.HasValue)
                {
                    throw new InvalidOperationException("Player template must have a PlayerClass");
                }

                character = Character.CreatePlayer(
                    template.Name,
                    template.PlayerClass.Value,
                    template.BaseStats);
            }
            else
            {
                if (!template.NPCBehavior.HasValue)
                {
                    throw new InvalidOperationException("NPC template must have NPCBehavior");
                }

                character = Character.CreateNPC(
                    template.Name,
                    template.NPCBehavior.Value,
                    template.BaseStats,
                    template.Id);
            }

            // Apply template configuration
            character.ApplyTemplate(template);

            return character;
        }

        /// <summary>
        /// Creates a player character with a specific class
        /// </summary>
        public Character CreatePlayer(string name, PlayerClass playerClass)
        {
            var baseStats = GetBaseStatsForPlayerClass(playerClass);
            return Character.CreatePlayer(name, playerClass, baseStats);
        }

        /// <summary>
        /// Creates an NPC with specific behavior
        /// </summary>
        public Character CreateNPC(string name, NPCBehavior behavior, CharacterStats? customStats = null)
        {
            var stats = customStats ?? GetDefaultNPCStats(behavior);
            return Character.CreateNPC(name, behavior, stats);
        }

        /// <summary>
        /// Creates multiple characters from template (useful for spawning enemies)
        /// </summary>
        public async Task<Character[]> CreateMultipleFromTemplateAsync(Guid templateId, int count)
        {
            var template = await _templateRepository.GetByIdAsync(templateId);
            if (template == null)
            {
                throw new ArgumentException($"Template with ID {templateId} not found");
            }

            var characters = new Character[count];
            for (int i = 0; i < count; i++)
            {
                characters[i] = await CreateFromTemplateAsync(template);
            }

            return characters;
        }

        // Helper methods for base stats
        private static CharacterStats GetBaseStatsForPlayerClass(PlayerClass playerClass)
        {
            return playerClass switch
            {
                PlayerClass.Warrior => new CharacterStats(1, 120, 15, 8, 8, 3),
                PlayerClass.Mage => new CharacterStats(1, 80, 6, 3, 12, 15),
                PlayerClass.Rogue => new CharacterStats(1, 90, 10, 5, 15, 6),
                PlayerClass.Archer => new CharacterStats(1, 85, 12, 4, 14, 8),
                PlayerClass.Paladin => new CharacterStats(1, 110, 12, 10, 6, 10),
                PlayerClass.Necromancer => new CharacterStats(1, 70, 5, 2, 10, 18),
                _ => new CharacterStats() // Default stats
            };
        }

        private static CharacterStats GetDefaultNPCStats(NPCBehavior behavior)
        {
            return behavior switch
            {
                NPCBehavior.Aggressive => new CharacterStats(1, 100, 12, 6, 10, 5),
                NPCBehavior.Defensive => new CharacterStats(1, 120, 8, 12, 6, 5),
                NPCBehavior.Friendly => new CharacterStats(1, 80, 5, 3, 8, 3),
                NPCBehavior.Vendor => new CharacterStats(1, 60, 3, 2, 5, 2),
                NPCBehavior.QuestGiver => new CharacterStats(1, 70, 4, 3, 6, 8),
                NPCBehavior.Guard => new CharacterStats(1, 150, 14, 15, 8, 4),
                NPCBehavior.Patrol => new CharacterStats(1, 90, 10, 8, 12, 5),
                _ => new CharacterStats()
            };
        }

        // Legacy support - creates enemy from EnemyTemplate
        public async Task<Character> CreateEnemyFromTemplate(EnemyTemplate enemyTemplate)
        {
            // Convert old EnemyTemplate to new CharacterTemplate approach
            var stats = new CharacterStats(
                1,
                enemyTemplate.BaseHealth,
                enemyTemplate.BaseStrength,
                enemyTemplate.BaseDefense,
                10, // Default speed
                5   // Default magic
            );

            var character = Character.CreateNPC(
                enemyTemplate.Name,
                NPCBehavior.Aggressive, // Default to aggressive for enemies
                stats,
                enemyTemplate.Id
            );

            // Apply old template data
            character.CustomData["ExperienceReward"] = enemyTemplate.ExperienceReward;
            character.CustomData["EnemyType"] = enemyTemplate.EnemyType.ToString();
            character.CustomData["Description"] = enemyTemplate.Description;
            character.CustomData["AttackPattern"] = enemyTemplate.AttackPattern;
            
            foreach (var ability in enemyTemplate.SpecialAbilities)
            {
                character.CustomData[$"Ability_{ability.Key}"] = ability.Value;
            }

            return character;
        }
    }
}