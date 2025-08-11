using RpgGame.Domain.Entities.Configuration;
using RpgGame.Domain.ValueObjects;
using RpgGame.Domain.Enums;
using System;
using System.Collections.Generic;
using Xunit;

namespace RpgGame.UnitTests.Domain.Entities
{
    public class CharacterTemplateTests
    {
        private CharacterStats CreateDefaultStats()
        {
            return new CharacterStats(
                level: 1,
                maxHealth: 100,
                strength: 10,
                defense: 8,
                speed: 12,
                magic: 5
            );
        }

        [Fact]
        public void CreateNPCTemplate_ValidParameters_CreatesTemplate()
        {
            // Arrange
            var name = "Goblin Warrior";
            var description = "A fierce goblin warrior";
            var characterType = CharacterType.NPC;
            var baseStats = CreateDefaultStats();
            var abilityIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            // Act
            var template = CharacterTemplate.CreateNPCTemplate(
                name, description, baseStats, abilityIds);

            // Assert
            Assert.Equal(name, template.Name);
            Assert.Equal(description, template.Description);
            Assert.Equal(characterType, template.CharacterType);
            Assert.Equal(baseStats, template.BaseStats);
            Assert.Equal(abilityIds, template.AbilityIds);
            Assert.NotNull(template.ConfigurationData);
            Assert.Empty(template.ConfigurationData);
        }

        [Fact]
        public void CreatePlayerTemplate_ValidParameters_CreatesTemplate()
        {
            // Arrange
            var name = "Warrior Starter";
            var description = "Basic warrior template";
            var characterType = CharacterType.Player;
            var baseStats = CreateDefaultStats();
            var abilityIds = new List<Guid> { Guid.NewGuid() };

            // Act
            var template = CharacterTemplate.CreatePlayerTemplate(
                name, description, baseStats, abilityIds);

            // Assert
            Assert.Equal(name, template.Name);
            Assert.Equal(description, template.Description);
            Assert.Equal(characterType, template.CharacterType);
            Assert.Equal(baseStats, template.BaseStats);
            Assert.Equal(abilityIds, template.AbilityIds);
            Assert.NotNull(template.ConfigurationData);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void CreateNPCTemplate_InvalidName_ThrowsArgumentException(string invalidName)
        {
            // Arrange
            var baseStats = CreateDefaultStats();
            var abilityIds = new List<Guid>();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                CharacterTemplate.CreateNPCTemplate(invalidName, "Description", baseStats, abilityIds));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void CreateNPCTemplate_InvalidDescription_ThrowsArgumentException(string invalidDescription)
        {
            // Arrange
            var baseStats = CreateDefaultStats();
            var abilityIds = new List<Guid>();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                CharacterTemplate.CreateNPCTemplate("Valid Name", invalidDescription, baseStats, abilityIds));
        }

        [Fact]
        public void CreateNPCTemplate_NullStats_ThrowsArgumentNullException()
        {
            // Arrange
            var abilityIds = new List<Guid>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                CharacterTemplate.CreateNPCTemplate("Name", "Description", null, abilityIds));
        }

        [Fact]
        public void CreateNPCTemplate_NullAbilityIds_ThrowsArgumentNullException()
        {
            // Arrange
            var baseStats = CreateDefaultStats();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                CharacterTemplate.CreateNPCTemplate("Name", "Description", baseStats, null));
        }

        [Fact]
        public void AddConfigurationData_ValidKeyValue_AddsData()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate(
                "Test NPC", "Description", CreateDefaultStats(), new List<Guid>());
            var key = "AggressionLevel";
            var value = "High";

            // Act
            template.AddConfiguration(key, value);

            // Assert
            Assert.True(template.ConfigurationData.ContainsKey(key));
            Assert.Equal(value, template.ConfigurationData[key]);
        }

        [Fact]
        public void AddConfigurationData_DuplicateKey_ThrowsInvalidOperationException()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate(
                "Test NPC", "Description", CreateDefaultStats(), new List<Guid>());
            var key = "AggressionLevel";
            template.AddConfiguration(key, "High");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                template.AddConfiguration(key, "Low"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void AddConfigurationData_InvalidKey_ThrowsArgumentException(string invalidKey)
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate(
                "Test NPC", "Description", CreateDefaultStats(), new List<Guid>());

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                template.AddConfiguration(invalidKey, "Value"));
        }

        [Fact]
        public void RemoveConfigurationData_ExistingKey_RemovesData()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate(
                "Test NPC", "Description", CreateDefaultStats(), new List<Guid>());
            var key = "AggressionLevel";
            template.AddConfiguration(key, "High");

            // Act
            var result = template.RemoveConfiguration(key);

            // Assert
            Assert.True(result);
            Assert.False(template.ConfigurationData.ContainsKey(key));
        }

        [Fact]
        public void RemoveConfigurationData_NonExistingKey_ReturnsFalse()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate(
                "Test NPC", "Description", CreateDefaultStats(), new List<Guid>());

            // Act
            var result = template.RemoveConfiguration("NonExistentKey");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void AddAbility_ValidAbilityId_AddsToList()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate(
                "Test NPC", "Description", CreateDefaultStats(), new List<Guid>());
            var abilityId = Guid.NewGuid();

            // Act
            template.AddAbility(abilityId);

            // Assert
            Assert.Contains(abilityId, template.AbilityIds);
        }

        [Fact]
        public void AddAbility_DuplicateAbilityId_ThrowsInvalidOperationException()
        {
            // Arrange
            var abilityId = Guid.NewGuid();
            var template = CharacterTemplate.CreateNPCTemplate(
                "Test NPC", "Description", CreateDefaultStats(), new List<Guid> { abilityId });

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => template.AddAbility(abilityId));
        }

        [Fact]
        public void AddAbility_EmptyGuid_ThrowsArgumentException()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate(
                "Test NPC", "Description", CreateDefaultStats(), new List<Guid>());

            // Act & Assert
            Assert.Throws<ArgumentException>(() => template.AddAbility(Guid.Empty));
        }

        [Fact]
        public void RemoveAbility_ExistingAbilityId_RemovesFromList()
        {
            // Arrange
            var abilityId = Guid.NewGuid();
            var template = CharacterTemplate.CreateNPCTemplate(
                "Test NPC", "Description", CreateDefaultStats(), new List<Guid> { abilityId });

            // Act
            var result = template.RemoveAbility(abilityId);

            // Assert
            Assert.True(result);
            Assert.DoesNotContain(abilityId, template.AbilityIds);
        }

        [Fact]
        public void RemoveAbility_NonExistingAbilityId_ReturnsFalse()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate(
                "Test NPC", "Description", CreateDefaultStats(), new List<Guid>());

            // Act
            var result = template.RemoveAbility(Guid.NewGuid());

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void UpdateDetails_ValidParameters_UpdatesProperties()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate(
                "Old Name", "Old Description", CreateDefaultStats(), new List<Guid>());
            var newName = "New Name";
            var newDescription = "New Description";
            var newStats = new CharacterStats(2, 150, 15, 12, 18, 8);

            // Act
            template.UpdateDetails(newName, newDescription, newStats);

            // Assert
            Assert.Equal(newName, template.Name);
            Assert.Equal(newDescription, template.Description);
            Assert.Equal(newStats, template.BaseStats);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void UpdateDetails_InvalidName_ThrowsArgumentException(string invalidName)
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate(
                "Name", "Description", CreateDefaultStats(), new List<Guid>());

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                template.UpdateDetails(invalidName, "New Description", CreateDefaultStats()));
        }

        [Fact]
        public void UpdateDetails_NullStats_ThrowsArgumentNullException()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate(
                "Name", "Description", CreateDefaultStats(), new List<Guid>());

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                template.UpdateDetails("New Name", "New Description", null));
        }

        [Fact]
        public void ClearConfiguration_RemovesAllConfigurationData()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate(
                "Test NPC", "Description", CreateDefaultStats(), new List<Guid>());
            template.AddConfiguration("Key1", "Value1");
            template.AddConfiguration("Key2", "Value2");

            // Act
            template.ClearConfiguration();

            // Assert
            Assert.Empty(template.ConfigurationData);
        }

        [Fact]
        public void ClearAbilities_RemovesAllAbilities()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate(
                "Test NPC", "Description", CreateDefaultStats(), 
                new List<Guid> { Guid.NewGuid(), Guid.NewGuid() });

            // Act
            template.ClearAbilities();

            // Assert
            Assert.Empty(template.AbilityIds);
        }

        [Fact]
        public void ComplexScenario_NPCTemplateWithFullConfiguration()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate(
                "Fire Dragon", "A powerful fire-breathing dragon", 
                new CharacterStats(50, 2000, 100, 80, 60, 150), 
                new List<Guid>());

            // Act - Build complex configuration
            var fireBreathAbility = Guid.NewGuid();
            var clawAttackAbility = Guid.NewGuid();
            
            template.AddAbility(fireBreathAbility);
            template.AddAbility(clawAttackAbility);
            template.AddConfiguration("Element", "Fire");
            template.AddConfiguration("FlightCapable", true);
            template.AddConfiguration("TreasureHoard", 10000);
            template.AddConfiguration("AggressionLevel", "Extreme");

            // Assert
            Assert.Equal("Fire Dragon", template.Name);
            Assert.Equal(CharacterType.NPC, template.CharacterType);
            Assert.Equal(2, template.AbilityIds.Count);
            Assert.Equal(4, template.ConfigurationData.Count);
            Assert.Equal("Fire", template.ConfigurationData["Element"]);
            Assert.Equal(true, template.ConfigurationData["FlightCapable"]);
            Assert.Equal(10000, template.ConfigurationData["TreasureHoard"]);
            Assert.Equal("Extreme", template.ConfigurationData["AggressionLevel"]);
        }
    }
}