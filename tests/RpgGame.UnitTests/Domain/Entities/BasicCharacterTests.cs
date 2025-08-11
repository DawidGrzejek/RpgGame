using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.ValueObjects;
using RpgGame.Domain.Enums;
using System;
using Xunit;

namespace RpgGame.UnitTests.Domain.Entities
{
    /// <summary>
    /// Basic tests for the new Character system that don't require external dependencies
    /// These tests verify the core functionality is working correctly
    /// </summary>
    public class BasicCharacterTests
    {
        private CharacterStats CreateWarriorStats()
        {
            return new CharacterStats(
                level: 1,
                maxHealth: 120,
                strength: 15,
                defense: 12,
                speed: 10,
                magic: 5
            );
        }

        private CharacterStats CreateMageStats()
        {
            return new CharacterStats(
                level: 1,
                maxHealth: 80,
                strength: 6,
                defense: 8,
                speed: 14,
                magic: 18
            );
        }

        [Fact]
        public void CharacterStats_Construction_WorksCorrectly()
        {
            // Arrange & Act
            var stats = new CharacterStats(5, 150, 20, 15, 18, 12);

            // Assert
            Assert.Equal(5, stats.Level);
            Assert.Equal(150, stats.MaxHealth);
            Assert.Equal(150, stats.CurrentHealth); // Should start at max
            Assert.Equal(20, stats.Strength);
            Assert.Equal(15, stats.Defense);
            Assert.Equal(18, stats.Speed);
            Assert.Equal(12, stats.Magic);
        }

        [Fact]
        public void Character_CreatePlayer_WorksCorrectly()
        {
            // Arrange
            var name = "TestWarrior";
            var playerClass = PlayerClass.Warrior;
            var stats = CreateWarriorStats();

            // Act
            var character = Character.CreatePlayer(name, playerClass, stats);

            // Assert
            Assert.Equal(name, character.Name);
            Assert.Equal(CharacterType.Player, character.Type);
            Assert.Equal(playerClass, character.PlayerClass);
            Assert.Equal(stats, character.Stats);
            Assert.Null(character.NPCBehavior);
            Assert.True(character.IsAlive);
            Assert.NotNull(character.CustomData);
            Assert.Empty(character.CustomData);
            Assert.Equal(0, character.Experience);
        }

        [Fact]
        public void Character_CreateNPC_WorksCorrectly()
        {
            // Arrange
            var name = "Goblin Scout";
            var behavior = NPCBehavior.Aggressive;
            var stats = new CharacterStats(3, 60, 12, 8, 15, 3);

            // Act
            var character = Character.CreateNPC(name, behavior, stats);

            // Assert
            Assert.Equal(name, character.Name);
            Assert.Equal(CharacterType.NPC, character.Type);
            Assert.Equal(behavior, character.NPCBehavior);
            Assert.Equal(stats, character.Stats);
            Assert.Null(character.PlayerClass);
            Assert.True(character.IsAlive);
            Assert.NotNull(character.CustomData);
        }

        [Fact]
        public void Character_TakeDamage_ReducesHealthWithDefenseCalculation()
        {
            // Arrange
            var character = Character.CreatePlayer("TestPlayer", PlayerClass.Warrior, CreateWarriorStats());
            var damage = 30;
            var initialHealth = character.Stats.CurrentHealth;
            var defense = character.Stats.Defense;

            // Act
            character.TakeDamage(damage);

            // Assert
            // Actual damage = max(1, damage - defense) = max(1, 30-12) = 18
            var expectedDamage = Math.Max(1, damage - defense);
            var expectedHealth = initialHealth - expectedDamage;
            Assert.Equal(expectedHealth, character.Stats.CurrentHealth);
            Assert.True(character.IsAlive);
        }

        [Fact]
        public void Character_TakeFatalDamage_Dies()
        {
            // Arrange
            var character = Character.CreatePlayer("TestPlayer", PlayerClass.Mage, CreateMageStats());

            // Act
            character.TakeDamage(200); // Much more than health + defense

            // Assert
            Assert.Equal(0, character.Stats.CurrentHealth);
            Assert.False(character.IsAlive);
        }

        [Fact]
        public void Character_TakeDamage_NegativeDamage_ThrowsException()
        {
            // Arrange
            var character = Character.CreatePlayer("TestPlayer", PlayerClass.Warrior, CreateWarriorStats());

            // Act & Assert
            Assert.Throws<ArgumentException>(() => character.TakeDamage(-10));
        }

        [Fact]
        public void Character_Heal_WorksCorrectly()
        {
            // Arrange
            var character = Character.CreatePlayer("TestPlayer", PlayerClass.Warrior, CreateWarriorStats());
            character.TakeDamage(50); // Damage first
            var healthAfterDamage = character.Stats.CurrentHealth;
            var healAmount = 30;

            // Act
            character.Heal(healAmount);

            // Assert
            var expectedHealth = Math.Min(character.Stats.MaxHealth, healthAfterDamage + healAmount);
            Assert.Equal(expectedHealth, character.Stats.CurrentHealth);
        }

        [Fact]
        public void Character_Heal_CapsAtMaxHealth()
        {
            // Arrange
            var character = Character.CreatePlayer("TestPlayer", PlayerClass.Warrior, CreateWarriorStats());
            character.TakeDamage(20); // Damage first
            var healAmount = 200; // Much more than needed

            // Act
            character.Heal(healAmount);

            // Assert
            Assert.Equal(character.Stats.MaxHealth, character.Stats.CurrentHealth);
        }

        [Fact]
        public void Character_Heal_DeadCharacter_DoesNothing()
        {
            // Arrange
            var character = Character.CreatePlayer("TestPlayer", PlayerClass.Warrior, CreateWarriorStats());
            character.TakeDamage(200); // Kill the character
            Assert.False(character.IsAlive);

            // Act
            character.Heal(50);

            // Assert
            Assert.Equal(0, character.Stats.CurrentHealth);
            Assert.False(character.IsAlive);
        }

        [Fact]
        public void Character_Heal_NegativeAmount_ThrowsException()
        {
            // Arrange
            var character = Character.CreatePlayer("TestPlayer", PlayerClass.Warrior, CreateWarriorStats());

            // Act & Assert
            Assert.Throws<ArgumentException>(() => character.Heal(-10));
        }

        [Fact]
        public void Character_GainExperience_IncreasesExperience()
        {
            // Arrange
            var character = Character.CreatePlayer("TestPlayer", PlayerClass.Warrior, CreateWarriorStats());
            var expGain = 50;

            // Act
            character.GainExperience(expGain);

            // Assert
            Assert.Equal(50, character.Experience);
        }

        [Fact]
        public void Character_GainExperience_EnoughForLevelUp_LevelsUp()
        {
            // Arrange
            var character = Character.CreatePlayer("TestPlayer", PlayerClass.Warrior, CreateWarriorStats());
            var expForLevelUp = 1000; // Level * 1000, so 1000 for level 2
            var initialLevel = character.Stats.Level;

            // Act
            character.GainExperience(expForLevelUp);

            // Assert
            Assert.Equal(initialLevel + 1, character.Stats.Level);
            Assert.Equal(0, character.Experience); // XP resets after level up
        }

        [Fact]
        public void Character_GainExperience_NPCDoesNotGainExperience()
        {
            // Arrange
            var npc = Character.CreateNPC("Test NPC", NPCBehavior.Aggressive, CreateWarriorStats());

            // Act
            npc.GainExperience(100);

            // Assert
            Assert.Equal(0, npc.Experience); // NPCs don't gain experience
        }

        [Fact]
        public void Character_CustomData_CanStoreValues()
        {
            // Arrange
            var character = Character.CreatePlayer("TestMage", PlayerClass.Mage, CreateMageStats());

            // Act
            character.CustomData["Mana"] = 100;
            character.CustomData["School"] = "Evocation";
            character.CustomData["CriticalChance"] = 0.15;

            // Assert
            Assert.Equal(100, character.CustomData["Mana"]);
            Assert.Equal("Evocation", character.CustomData["School"]);
            Assert.Equal(0.15, character.CustomData["CriticalChance"]);
        }

        [Fact]
        public void Character_PlayerVsNPC_BehavesCorrectly()
        {
            // Arrange & Act
            var player = Character.CreatePlayer("Hero", PlayerClass.Paladin, CreateWarriorStats());
            var npc = Character.CreateNPC("Merchant", NPCBehavior.Vendor, CreateMageStats());

            // Assert Player
            Assert.Equal(CharacterType.Player, player.Type);
            Assert.Equal(PlayerClass.Paladin, player.PlayerClass);
            Assert.Null(player.NPCBehavior);

            // Assert NPC
            Assert.Equal(CharacterType.NPC, npc.Type);
            Assert.Equal(NPCBehavior.Vendor, npc.NPCBehavior);
            Assert.Null(npc.PlayerClass);
        }

        [Fact]
        public void Character_ExportState_ReturnsCorrectData()
        {
            // Arrange
            var character = Character.CreatePlayer("TestHero", PlayerClass.Archer, CreateWarriorStats());
            character.GainExperience(25);
            character.TakeDamage(10);

            // Act
            var state = character.ExportState();

            // Assert
            Assert.NotNull(state);
            Assert.Equal(character.Id, state.AggregateId);
            Assert.Equal("TestHero", state.Name);
            Assert.Equal(character.Stats.CurrentHealth, state.Health);
            Assert.Equal(character.Stats.MaxHealth, state.MaxHealth);
            Assert.Equal(character.Stats.Level, state.Level);
        }

        [Theory]
        [InlineData(PlayerClass.Warrior)]
        [InlineData(PlayerClass.Mage)]
        [InlineData(PlayerClass.Rogue)]
        [InlineData(PlayerClass.Archer)]
        [InlineData(PlayerClass.Paladin)]
        [InlineData(PlayerClass.Necromancer)]
        public void Character_AllPlayerClasses_CanBeCreated(PlayerClass playerClass)
        {
            // Arrange
            var name = $"Test{playerClass}";
            var stats = CreateWarriorStats();

            // Act
            var character = Character.CreatePlayer(name, playerClass, stats);

            // Assert
            Assert.Equal(playerClass, character.PlayerClass);
            Assert.Equal(CharacterType.Player, character.Type);
        }

        [Theory]
        [InlineData(NPCBehavior.Aggressive)]
        [InlineData(NPCBehavior.Defensive)]
        [InlineData(NPCBehavior.Passive)]
        [InlineData(NPCBehavior.Friendly)]
        [InlineData(NPCBehavior.Vendor)]
        [InlineData(NPCBehavior.QuestGiver)]
        [InlineData(NPCBehavior.Guard)]
        [InlineData(NPCBehavior.Patrol)]
        public void Character_AllNPCBehaviors_CanBeCreated(NPCBehavior behavior)
        {
            // Arrange
            var name = $"Test{behavior}";
            var stats = CreateMageStats();

            // Act
            var character = Character.CreateNPC(name, behavior, stats);

            // Assert
            Assert.Equal(behavior, character.NPCBehavior);
            Assert.Equal(CharacterType.NPC, character.Type);
        }

        [Fact]
        public void Character_AddAbility_WorksCorrectly()
        {
            // Arrange
            var character = Character.CreatePlayer("TestPlayer", PlayerClass.Mage, CreateMageStats());
            var abilityId = Guid.NewGuid();

            // Act
            character.AddAbility(abilityId);

            // Assert
            Assert.Contains(abilityId, character.Abilities);
        }

        [Fact]
        public void Character_CompatibilityProperties_WorkCorrectly()
        {
            // Arrange
            var stats = CreateWarriorStats();
            var character = Character.CreatePlayer("TestPlayer", PlayerClass.Warrior, stats);

            // Act & Assert
            Assert.Equal(stats.CurrentHealth, character.Health);
            Assert.Equal(stats.MaxHealth, character.MaxHealth);
            Assert.Equal(stats.Level, character.Level);
            Assert.Equal(stats.Strength, character.Strength);
            Assert.Equal(stats.Defense, character.Defense);
        }

        [Fact]
        public void NewArchitecture_ReplacesOldInheritanceSystem()
        {
            // This test demonstrates that we can create all character types with one unified system
            
            // Old system would need: Warrior, Mage, Rogue, Enemy, Vendor, QuestGiver classes
            // New system uses: One Character class + different creation parameters

            // Create what used to be different classes
            var warrior = Character.CreatePlayer("Sir Lancelot", PlayerClass.Warrior, CreateWarriorStats());
            var mage = Character.CreatePlayer("Gandalf", PlayerClass.Mage, CreateMageStats());
            var enemy = Character.CreateNPC("Orc Warrior", NPCBehavior.Aggressive, CreateWarriorStats());
            var vendor = Character.CreateNPC("Shopkeeper", NPCBehavior.Vendor, CreateMageStats());

            // All are the same type now, but behave differently based on their properties
            Assert.IsType<Character>(warrior);
            Assert.IsType<Character>(mage);
            Assert.IsType<Character>(enemy);
            Assert.IsType<Character>(vendor);

            // But they have different characteristics
            Assert.Equal(PlayerClass.Warrior, warrior.PlayerClass);
            Assert.Equal(PlayerClass.Mage, mage.PlayerClass);
            Assert.Equal(NPCBehavior.Aggressive, enemy.NPCBehavior);
            Assert.Equal(NPCBehavior.Vendor, vendor.NPCBehavior);
        }
    }
}