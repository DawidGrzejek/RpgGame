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

        #region Constructor Tests - Extended Edge Cases

        [Fact]
        public void Constructor_ValidParameters_InitializesAllProperties()
        {
            // Arrange
            var name = "Test Template";
            var description = "Test Description";
            var characterType = CharacterType.Player;
            var baseStats = CreateDefaultStats();
            var npcBehavior = NPCBehavior.Aggressive;
            var playerClass = PlayerClass.Mage;

            // Act
            var template = new CharacterTemplate(name, description, characterType, baseStats, npcBehavior, playerClass);

            // Assert
            Assert.Equal(name, template.Name);
            Assert.Equal(description, template.Description);
            Assert.Equal(characterType, template.CharacterType);
            Assert.Equal(baseStats, template.BaseStats);
            Assert.Equal(npcBehavior, template.NPCBehavior);
            Assert.Equal(playerClass, template.PlayerClass);
            Assert.NotNull(template.ConfigurationData);
            Assert.NotNull(template.AbilityIds);
            Assert.NotNull(template.LootTableIds);
            Assert.NotNull(template.BehaviorData);
            Assert.Empty(template.ConfigurationData);
            Assert.Empty(template.AbilityIds);
            Assert.Empty(template.LootTableIds);
            Assert.Empty(template.BehaviorData);
        }

        [Fact]
        public void Constructor_NullOptionalParameters_InitializesCorrectly()
        {
            // Arrange
            var name = "Test Template";
            var description = "Test Description";
            var characterType = CharacterType.NPC;
            var baseStats = CreateDefaultStats();

            // Act
            var template = new CharacterTemplate(name, description, characterType, baseStats);

            // Assert
            Assert.Equal(name, template.Name);
            Assert.Equal(description, template.Description);
            Assert.Equal(characterType, template.CharacterType);
            Assert.Equal(baseStats, template.BaseStats);
            Assert.Null(template.NPCBehavior);
            Assert.Null(template.PlayerClass);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_EmptyOrWhitespaceNameButNotNull_DoesNotThrow(string name)
        {
            // Arrange
            var description = "Test Description";
            var characterType = CharacterType.Player;
            var baseStats = CreateDefaultStats();

            // Act & Assert - Constructor allows empty/whitespace names unlike factory methods
            var template = new CharacterTemplate(name, description, characterType, baseStats);
            Assert.Equal(name, template.Name);
        }

        [Fact]
        public void Constructor_NullName_ThrowsArgumentNullException()
        {
            // Arrange
            var description = "Test Description";
            var characterType = CharacterType.Player;
            var baseStats = CreateDefaultStats();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new CharacterTemplate(null, description, characterType, baseStats));
        }

        [Fact]
        public void Constructor_NullDescription_ThrowsArgumentNullException()
        {
            // Arrange
            var name = "Test Name";
            var characterType = CharacterType.Player;
            var baseStats = CreateDefaultStats();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new CharacterTemplate(name, null, characterType, baseStats));
        }

        [Fact]
        public void Constructor_NullBaseStats_ThrowsArgumentNullException()
        {
            // Arrange
            var name = "Test Name";
            var description = "Test Description";
            var characterType = CharacterType.Player;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new CharacterTemplate(name, description, characterType, null));
        }

        #endregion

        #region Factory Method Tests - Extended Coverage

        [Fact]
        public void CreatePlayerTemplate_WithAllPlayerClasses_CreatesCorrectTemplate()
        {
            // Arrange
            var name = "Test Player";
            var description = "Test Description";
            var baseStats = CreateDefaultStats();
            var abilityIds = new List<Guid> { Guid.NewGuid() };

            var playerClasses = new[] { PlayerClass.Warrior, PlayerClass.Mage, PlayerClass.Rogue, 
                                       PlayerClass.Archer, PlayerClass.Paladin, PlayerClass.Necromancer };

            foreach (var playerClass in playerClasses)
            {
                // Act
                var template = CharacterTemplate.CreatePlayerTemplate(name, description, baseStats, abilityIds, playerClass);

                // Assert
                Assert.Equal(CharacterType.Player, template.CharacterType);
                Assert.Equal(playerClass, template.PlayerClass);
                Assert.Null(template.NPCBehavior);
            }
        }

        [Fact]
        public void CreatePlayerTemplate_DefaultPlayerClass_UsesWarrior()
        {
            // Arrange
            var name = "Default Player";
            var description = "Test Description";
            var baseStats = CreateDefaultStats();
            var abilityIds = new List<Guid>();

            // Act
            var template = CharacterTemplate.CreatePlayerTemplate(name, description, baseStats, abilityIds);

            // Assert
            Assert.Equal(PlayerClass.Warrior, template.PlayerClass);
        }

        [Fact]
        public void CreatePlayerTemplate_EmptyAbilityList_DoesNotThrow()
        {
            // Arrange
            var name = "Player";
            var description = "Description";
            var baseStats = CreateDefaultStats();
            var abilityIds = new List<Guid>();

            // Act
            var template = CharacterTemplate.CreatePlayerTemplate(name, description, baseStats, abilityIds);

            // Assert
            Assert.Empty(template.AbilityIds);
        }

        [Fact]
        public void CreateNPCTemplate_WithAllNPCBehaviors_CreatesCorrectTemplate()
        {
            // Arrange
            var name = "Test NPC";
            var description = "Test Description";
            var baseStats = CreateDefaultStats();
            var abilityIds = new List<Guid>();

            var npcBehaviors = new[] { NPCBehavior.Aggressive, NPCBehavior.Defensive, NPCBehavior.Passive,
                                      NPCBehavior.Friendly, NPCBehavior.Vendor, NPCBehavior.QuestGiver,
                                      NPCBehavior.Guard, NPCBehavior.Patrol };

            foreach (var npcBehavior in npcBehaviors)
            {
                // Act
                var template = CharacterTemplate.CreateNPCTemplate(name, description, baseStats, abilityIds, npcBehavior);

                // Assert
                Assert.Equal(CharacterType.NPC, template.CharacterType);
                Assert.Equal(npcBehavior, template.NPCBehavior);
                Assert.Null(template.PlayerClass);
            }
        }

        [Fact]
        public void CreateNPCTemplate_DefaultBehavior_UsesPassive()
        {
            // Arrange
            var name = "Default NPC";
            var description = "Test Description";
            var baseStats = CreateDefaultStats();
            var abilityIds = new List<Guid>();

            // Act
            var template = CharacterTemplate.CreateNPCTemplate(name, description, baseStats, abilityIds);

            // Assert
            Assert.Equal(NPCBehavior.Passive, template.NPCBehavior);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void CreatePlayerTemplate_InvalidName_ThrowsArgumentException(string invalidName)
        {
            // Arrange
            var baseStats = CreateDefaultStats();
            var abilityIds = new List<Guid>();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                CharacterTemplate.CreatePlayerTemplate(invalidName, "Description", baseStats, abilityIds));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void CreatePlayerTemplate_InvalidDescription_ThrowsArgumentException(string invalidDescription)
        {
            // Arrange
            var baseStats = CreateDefaultStats();
            var abilityIds = new List<Guid>();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                CharacterTemplate.CreatePlayerTemplate("Name", invalidDescription, baseStats, abilityIds));
        }

        [Fact]
        public void CreatePlayerTemplate_NullBaseStats_ThrowsArgumentNullException()
        {
            // Arrange
            var abilityIds = new List<Guid>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                CharacterTemplate.CreatePlayerTemplate("Name", "Description", null, abilityIds));
        }

        [Fact]
        public void CreatePlayerTemplate_NullAbilityIds_ThrowsArgumentNullException()
        {
            // Arrange
            var baseStats = CreateDefaultStats();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                CharacterTemplate.CreatePlayerTemplate("Name", "Description", baseStats, null));
        }

        [Fact]
        public void CreateEnemyTemplate_ValidParametersWithLoot_CreatesCorrectTemplate()
        {
            // Arrange
            var name = "Dragon Boss";
            var description = "A powerful dragon";
            var baseStats = CreateDefaultStats();
            var abilityIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var lootTableIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var experienceReward = 1000;

            // Act
            var template = CharacterTemplate.CreateEnemyTemplate(name, description, baseStats, abilityIds, lootTableIds, experienceReward);

            // Assert
            Assert.Equal(name, template.Name);
            Assert.Equal(description, template.Description);
            Assert.Equal(CharacterType.NPC, template.CharacterType);
            Assert.Equal(NPCBehavior.Aggressive, template.NPCBehavior);
            Assert.Null(template.PlayerClass);
            Assert.Equal(baseStats, template.BaseStats);
            Assert.Equal(abilityIds, template.AbilityIds);
            Assert.Equal(lootTableIds, template.LootTableIds);
            Assert.Equal(experienceReward, template.GetConfiguration<int>("ExperienceReward"));
            Assert.True(template.GetConfiguration<bool>("IsEnemy"));
        }

        [Fact]
        public void CreateEnemyTemplate_NullOptionalParameters_CreatesValidTemplate()
        {
            // Arrange
            var name = "Simple Enemy";
            var description = "Basic enemy";
            var baseStats = CreateDefaultStats();

            // Act
            var template = CharacterTemplate.CreateEnemyTemplate(name, description, baseStats);

            // Assert
            Assert.Equal(CharacterType.NPC, template.CharacterType);
            Assert.Equal(NPCBehavior.Aggressive, template.NPCBehavior);
            Assert.Empty(template.AbilityIds);
            Assert.Empty(template.LootTableIds);
            Assert.Equal(0, template.GetConfiguration<int>("ExperienceReward"));
            Assert.True(template.GetConfiguration<bool>("IsEnemy"));
        }

        [Fact]
        public void CreateEnemyTemplate_EmptyLists_DoesNotAddItems()
        {
            // Arrange
            var name = "Empty Lists Enemy";
            var description = "Enemy with empty lists";
            var baseStats = CreateDefaultStats();
            var emptyAbilities = new List<Guid>();
            var emptyLoot = new List<Guid>();

            // Act
            var template = CharacterTemplate.CreateEnemyTemplate(name, description, baseStats, emptyAbilities, emptyLoot);

            // Assert
            Assert.Empty(template.AbilityIds);
            Assert.Empty(template.LootTableIds);
        }

        #endregion

        #region GetConfiguration Generic Method Tests

        [Fact]
        public void GetConfiguration_ExistingKeyWithCorrectType_ReturnsValue()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());
            template.AddConfiguration("TestString", "StringValue");
            template.AddConfiguration("TestInt", 42);
            template.AddConfiguration("TestBool", true);
            template.AddConfiguration("TestDouble", 3.14);

            // Act & Assert
            Assert.Equal("StringValue", template.GetConfiguration<string>("TestString"));
            Assert.Equal(42, template.GetConfiguration<int>("TestInt"));
            Assert.True(template.GetConfiguration<bool>("TestBool"));
            Assert.Equal(3.14, template.GetConfiguration<double>("TestDouble"));
        }

        [Fact]
        public void GetConfiguration_ExistingKeyWithWrongType_ReturnsDefault()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());
            template.AddConfiguration("TestInt", 42);

            // Act & Assert
            Assert.Equal(default(string), template.GetConfiguration<string>("TestInt"));
            Assert.Equal(default(bool), template.GetConfiguration<bool>("TestInt"));
            Assert.Equal(default(double), template.GetConfiguration<double>("TestInt"));
        }

        [Fact]
        public void GetConfiguration_NonExistingKey_ReturnsDefault()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());

            // Act & Assert
            Assert.Equal(default(string), template.GetConfiguration<string>("NonExistent"));
            Assert.Equal(default(int), template.GetConfiguration<int>("NonExistent"));
            Assert.False(template.GetConfiguration<bool>("NonExistent"));
        }

        [Fact]
        public void GetConfiguration_WithCustomDefaultValue_ReturnsCustomDefault()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());

            // Act & Assert
            Assert.Equal("CustomDefault", template.GetConfiguration("NonExistent", "CustomDefault"));
            Assert.Equal(99, template.GetConfiguration("NonExistent", 99));
            Assert.True(template.GetConfiguration("NonExistent", true));
        }

        [Fact]
        public void GetConfiguration_NullValue_ReturnsNull()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());
            template.AddConfiguration("NullValue", null);

            // Act & Assert
            Assert.Null(template.GetConfiguration<string>("NullValue"));
            Assert.Equal("DefaultValue", template.GetConfiguration("NullValue", "DefaultValue"));
        }

        [Fact]
        public void GetConfiguration_ComplexType_WorksCorrectly()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());
            var list = new List<string> { "item1", "item2" };
            template.AddConfiguration("TestList", list);

            // Act & Assert
            Assert.Equal(list, template.GetConfiguration<List<string>>("TestList"));
            Assert.Null(template.GetConfiguration<List<int>>("TestList")); // Wrong generic type
        }

        #endregion

        #region BehaviorData Tests

        [Fact]
        public void SetBehaviorData_ValidKeyValue_AddsData()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());
            var key = "PatrolRadius";
            var value = 50;

            // Act
            template.SetBehaviorData(key, value);

            // Assert
            Assert.True(template.BehaviorData.ContainsKey(key));
            Assert.Equal(value, template.BehaviorData[key]);
        }

        [Fact]
        public void SetBehaviorData_DuplicateKey_OverwritesValue()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());
            var key = "AggressionLevel";
            template.SetBehaviorData(key, "Low");

            // Act
            template.SetBehaviorData(key, "High");

            // Assert
            Assert.Equal("High", template.BehaviorData[key]);
        }

        [Fact]
        public void SetBehaviorData_NullKey_ThrowsException()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => template.SetBehaviorData(null, "Value"));
        }

        [Fact]
        public void SetBehaviorData_NullValue_AllowsNull()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());
            var key = "NullableValue";

            // Act
            template.SetBehaviorData(key, null);

            // Assert
            Assert.True(template.BehaviorData.ContainsKey(key));
            Assert.Null(template.BehaviorData[key]);
        }

        [Fact]
        public void GetBehaviorData_ExistingKeyWithCorrectType_ReturnsValue()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());
            template.SetBehaviorData("PatrolRadius", 100);
            template.SetBehaviorData("IsHostile", true);
            template.SetBehaviorData("GreetingMessage", "Hello traveler!");

            // Act & Assert
            Assert.Equal(100, template.GetBehaviorData<int>("PatrolRadius"));
            Assert.True(template.GetBehaviorData<bool>("IsHostile"));
            Assert.Equal("Hello traveler!", template.GetBehaviorData<string>("GreetingMessage"));
        }

        [Fact]
        public void GetBehaviorData_ExistingKeyWithWrongType_ReturnsDefault()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());
            template.SetBehaviorData("PatrolRadius", 100);

            // Act & Assert
            Assert.Equal(default(string), template.GetBehaviorData<string>("PatrolRadius"));
            Assert.Equal(default(bool), template.GetBehaviorData<bool>("PatrolRadius"));
        }

        [Fact]
        public void GetBehaviorData_NonExistingKey_ReturnsDefault()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());

            // Act & Assert
            Assert.Equal(default(int), template.GetBehaviorData<int>("NonExistent"));
            Assert.Equal(default(string), template.GetBehaviorData<string>("NonExistent"));
        }

        [Fact]
        public void GetBehaviorData_WithCustomDefault_ReturnsCustomDefault()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());

            // Act & Assert
            Assert.Equal(42, template.GetBehaviorData("NonExistent", 42));
            Assert.Equal("Default", template.GetBehaviorData("NonExistent", "Default"));
        }

        [Fact]
        public void RemoveBehaviorData_ExistingKey_RemovesAndReturnsTrue()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());
            var key = "TestKey";
            template.SetBehaviorData(key, "TestValue");

            // Act
            var result = template.RemoveBehaviorData(key);

            // Assert
            Assert.True(result);
            Assert.False(template.BehaviorData.ContainsKey(key));
        }

        [Fact]
        public void RemoveBehaviorData_NonExistingKey_ReturnsFalse()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());

            // Act
            var result = template.RemoveBehaviorData("NonExistent");

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Loot Management Tests

        [Fact]
        public void AddLootItem_ValidGuid_AddsToLootTable()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());
            var itemId = Guid.NewGuid();

            // Act
            template.AddLootItem(itemId);

            // Assert
            Assert.Contains(itemId, template.LootTableIds);
            Assert.Single(template.LootTableIds);
        }

        [Fact]
        public void AddLootItem_DuplicateGuid_DoesNotAddDuplicate()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());
            var itemId = Guid.NewGuid();
            template.AddLootItem(itemId);

            // Act
            template.AddLootItem(itemId);

            // Assert
            Assert.Single(template.LootTableIds);
            Assert.Contains(itemId, template.LootTableIds);
        }

        [Fact]
        public void AddLootItem_EmptyGuid_AddsEmptyGuid()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());

            // Act
            template.AddLootItem(Guid.Empty);

            // Assert
            Assert.Contains(Guid.Empty, template.LootTableIds);
        }

        [Fact]
        public void AddLootItem_MultipleItems_AddsAllItems()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());
            var item1 = Guid.NewGuid();
            var item2 = Guid.NewGuid();
            var item3 = Guid.NewGuid();

            // Act
            template.AddLootItem(item1);
            template.AddLootItem(item2);
            template.AddLootItem(item3);

            // Assert
            Assert.Equal(3, template.LootTableIds.Count);
            Assert.Contains(item1, template.LootTableIds);
            Assert.Contains(item2, template.LootTableIds);
            Assert.Contains(item3, template.LootTableIds);
        }

        [Fact]
        public void RemoveLootItem_ExistingItem_RemovesAndReturnsTrue()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());
            var itemId = Guid.NewGuid();
            template.AddLootItem(itemId);

            // Act
            var result = template.RemoveLootItem(itemId);

            // Assert
            Assert.True(result);
            Assert.DoesNotContain(itemId, template.LootTableIds);
        }

        [Fact]
        public void RemoveLootItem_NonExistingItem_ReturnsFalse()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());

            // Act
            var result = template.RemoveLootItem(Guid.NewGuid());

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void RemoveLootItem_EmptyGuid_WorksCorrectly()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());
            template.AddLootItem(Guid.Empty);

            // Act
            var result = template.RemoveLootItem(Guid.Empty);

            // Assert
            Assert.True(result);
            Assert.DoesNotContain(Guid.Empty, template.LootTableIds);
        }

        #endregion

        #region UpdateBaseStats Tests

        [Fact]
        public void UpdateBaseStats_ValidStats_UpdatesStats()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());
            var newStats = new CharacterStats(10, 500, 50, 30, 40, 25);

            // Act
            template.UpdateBaseStats(newStats);

            // Assert
            Assert.Equal(newStats, template.BaseStats);
        }

        [Fact]
        public void UpdateBaseStats_NullStats_ThrowsArgumentNullException()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => template.UpdateBaseStats(null));
        }

        [Fact]
        public void UpdateBaseStats_SameStats_UpdatesCorrectly()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());
            var sameStats = template.BaseStats;

            // Act
            template.UpdateBaseStats(sameStats);

            // Assert
            Assert.Equal(sameStats, template.BaseStats);
        }

        #endregion

        #region ClearConfiguration Tests - Extended

        [Fact]
        public void ClearConfiguration_WithExistingConfiguration_ClearsAndReturnsTrue()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());
            template.AddConfiguration("Key1", "Value1");
            template.AddConfiguration("Key2", 42);
            template.AddConfiguration("Key3", true);

            // Act
            var result = template.ClearConfiguration();

            // Assert
            Assert.True(result);
            Assert.Empty(template.ConfigurationData);
        }

        [Fact]
        public void ClearConfiguration_EmptyConfiguration_ReturnsFalse()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());

            // Act
            var result = template.ClearConfiguration();

            // Assert
            Assert.False(result);
            Assert.Empty(template.ConfigurationData);
        }

        [Fact]
        public void ClearConfiguration_CalledTwice_SecondCallReturnsFalse()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("NPC", "Description", CreateDefaultStats(), new List<Guid>());
            template.AddConfiguration("Key1", "Value1");

            // Act
            var firstResult = template.ClearConfiguration();
            var secondResult = template.ClearConfiguration();

            // Assert
            Assert.True(firstResult);
            Assert.False(secondResult);
            Assert.Empty(template.ConfigurationData);
        }

        #endregion

        #region Complex Integration Tests

        [Fact]
        public void IntegrationTest_FullPlayerTemplateLifecycle()
        {
            // Arrange
            var name = "Master Warrior";
            var description = "An experienced warrior ready for battle";
            var baseStats = new CharacterStats(20, 300, 45, 35, 25, 15);
            var abilityIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            // Act - Create player template
            var template = CharacterTemplate.CreatePlayerTemplate(name, description, baseStats, abilityIds, PlayerClass.Paladin);

            // Add configuration
            template.AddConfiguration("StartingGold", 1000);
            template.AddConfiguration("Reputation", "Noble");
            template.AddConfiguration("QuestCompleted", false);

            // Add behavior data
            template.SetBehaviorData("PreferredWeapon", "Sword and Shield");
            template.SetBehaviorData("LoyaltyLevel", 95);

            // Add more abilities
            var newAbility = Guid.NewGuid();
            template.AddAbility(newAbility);

            // Update some details
            var updatedStats = new CharacterStats(21, 320, 47, 37, 27, 17);
            template.UpdateDetails("Master Paladin Warrior", "A legendary warrior blessed by the light", updatedStats);

            // Assert - Verify final state
            Assert.Equal("Master Paladin Warrior", template.Name);
            Assert.Equal("A legendary warrior blessed by the light", template.Description);
            Assert.Equal(CharacterType.Player, template.CharacterType);
            Assert.Equal(PlayerClass.Paladin, template.PlayerClass);
            Assert.Equal(updatedStats, template.BaseStats);
            Assert.Equal(3, template.AbilityIds.Count); // Original 2 + 1 new
            Assert.Contains(newAbility, template.AbilityIds);

            // Verify configuration
            Assert.Equal(1000, template.GetConfiguration<int>("StartingGold"));
            Assert.Equal("Noble", template.GetConfiguration<string>("Reputation"));
            Assert.False(template.GetConfiguration<bool>("QuestCompleted"));

            // Verify behavior data
            Assert.Equal("Sword and Shield", template.GetBehaviorData<string>("PreferredWeapon"));
            Assert.Equal(95, template.GetBehaviorData<int>("LoyaltyLevel"));
        }

        [Fact]
        public void IntegrationTest_EnemyTemplateWithComplexLootSystem()
        {
            // Arrange
            var name = "Ancient Lich King";
            var description = "A powerful undead ruler with vast magical abilities";
            var baseStats = new CharacterStats(100, 5000, 80, 60, 30, 200);
            var abilityIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var lootIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

            // Act - Create enemy template with loot
            var template = CharacterTemplate.CreateEnemyTemplate(name, description, baseStats, abilityIds, lootIds, 50000);

            // Add more loot items
            var rareItem = Guid.NewGuid();
            var epicItem = Guid.NewGuid();
            template.AddLootItem(rareItem);
            template.AddLootItem(epicItem);

            // Add complex behavior data
            template.SetBehaviorData("PhaseTransitions", new List<int> { 75, 50, 25 }); // HP percentages for phase changes
            template.SetBehaviorData("MinionsToSummon", 5);
            template.SetBehaviorData("PreferredSpells", new[] { "Death Coil", "Bone Prison", "Soul Drain" });

            // Add additional configuration
            template.AddConfiguration("ImmuneToDeath", true);
            template.AddConfiguration("RegenerationRate", 50);
            template.AddConfiguration("MagicResistance", 0.8);

            // Modify loot - remove one item
            var removedLoot = lootIds[0];
            template.RemoveLootItem(removedLoot);

            // Assert - Verify complex state
            Assert.Equal(CharacterType.NPC, template.CharacterType);
            Assert.Equal(NPCBehavior.Aggressive, template.NPCBehavior);
            Assert.Equal(50000, template.GetConfiguration<int>("ExperienceReward"));
            Assert.True(template.GetConfiguration<bool>("IsEnemy"));
            Assert.True(template.GetConfiguration<bool>("ImmuneToDeath"));
            Assert.Equal(50, template.GetConfiguration<int>("RegenerationRate"));
            Assert.Equal(0.8, template.GetConfiguration<double>("MagicResistance"));

            // Verify loot table (original 4 - 1 removed + 2 added = 5)
            Assert.Equal(5, template.LootTableIds.Count);
            Assert.DoesNotContain(removedLoot, template.LootTableIds);
            Assert.Contains(rareItem, template.LootTableIds);
            Assert.Contains(epicItem, template.LootTableIds);

            // Verify complex behavior data
            Assert.Equal(new List<int> { 75, 50, 25 }, template.GetBehaviorData<List<int>>("PhaseTransitions"));
            Assert.Equal(5, template.GetBehaviorData<int>("MinionsToSummon"));
            var preferredSpells = template.GetBehaviorData<string[]>("PreferredSpells");
            Assert.Contains("Death Coil", preferredSpells);
            Assert.Contains("Bone Prison", preferredSpells);
            Assert.Contains("Soul Drain", preferredSpells);
        }

        [Fact]
        public void IntegrationTest_NPCTemplateConfigurationManipulation()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate(
                "Village Merchant", "A friendly trader", CreateDefaultStats(), new List<Guid>(), NPCBehavior.Vendor);

            // Act - Add various configuration types
            template.AddConfiguration("ShopInventory", new List<string> { "Potion", "Bread", "Map" });
            template.AddConfiguration("PriceMultiplier", 1.2);
            template.AddConfiguration("MaxBargainAttempts", 3);
            template.AddConfiguration("IsOpen", true);

            // Test configuration manipulation
            template.RemoveConfiguration("MaxBargainAttempts");
            
            // Add behavior data
            template.SetBehaviorData("GreetingPhrase", "Welcome to my shop!");
            template.SetBehaviorData("ClosingTime", TimeSpan.FromHours(18));

            // Test type-safe retrieval with defaults
            var inventory = template.GetConfiguration<List<string>>("ShopInventory");
            var multiplier = template.GetConfiguration<double>("PriceMultiplier");
            var attempts = template.GetConfiguration<int>("MaxBargainAttempts", 5); // Should return default
            var isOpen = template.GetConfiguration<bool>("IsOpen");
            var nonExistent = template.GetConfiguration<string>("NonExistent", "DefaultValue");

            // Assert
            Assert.Equal(NPCBehavior.Vendor, template.NPCBehavior);
            Assert.Equal(3, inventory.Count);
            Assert.Contains("Potion", inventory);
            Assert.Equal(1.2, multiplier);
            Assert.Equal(5, attempts); // Default value since we removed it
            Assert.True(isOpen);
            Assert.Equal("DefaultValue", nonExistent);
            Assert.Equal("Welcome to my shop!", template.GetBehaviorData<string>("GreetingPhrase"));
            Assert.Equal(TimeSpan.FromHours(18), template.GetBehaviorData<TimeSpan>("ClosingTime"));

            // Verify configuration count after removal
            Assert.Equal(3, template.ConfigurationData.Count); // 4 added - 1 removed = 3
            Assert.False(template.ConfigurationData.ContainsKey("MaxBargainAttempts"));
        }

        [Fact]
        public void IntegrationTest_TemplateDataPersistenceAfterUpdates()
        {
            // Arrange
            var originalStats = CreateDefaultStats();
            var template = CharacterTemplate.CreatePlayerTemplate("Test Player", "Test Description", originalStats, new List<Guid>());

            // Add initial data
            template.AddConfiguration("InitialKey", "InitialValue");
            template.SetBehaviorData("InitialBehavior", 100);
            var initialAbility = Guid.NewGuid();
            template.AddAbility(initialAbility);

            // Act - Perform updates
            var newStats = new CharacterStats(5, 200, 25, 20, 30, 15);
            template.UpdateDetails("Updated Player", "Updated Description", newStats);
            template.UpdateBaseStats(new CharacterStats(6, 220, 27, 22, 32, 17));

            // Add more data after updates
            template.AddConfiguration("PostUpdateKey", "PostUpdateValue");
            template.SetBehaviorData("PostUpdateBehavior", 200);
            var postUpdateAbility = Guid.NewGuid();
            template.AddAbility(postUpdateAbility);

            // Assert - Verify all data persisted correctly
            Assert.Equal("Updated Player", template.Name);
            Assert.Equal("Updated Description", template.Description);
            Assert.Equal(6, template.BaseStats.Level);
            Assert.Equal(220, template.BaseStats.MaxHealth);

            // Original data should persist
            Assert.Equal("InitialValue", template.GetConfiguration<string>("InitialKey"));
            Assert.Equal(100, template.GetBehaviorData<int>("InitialBehavior"));
            Assert.Contains(initialAbility, template.AbilityIds);

            // New data should be present
            Assert.Equal("PostUpdateValue", template.GetConfiguration<string>("PostUpdateKey"));
            Assert.Equal(200, template.GetBehaviorData<int>("PostUpdateBehavior"));
            Assert.Contains(postUpdateAbility, template.AbilityIds);

            // Verify counts
            Assert.Equal(2, template.ConfigurationData.Count);
            Assert.Equal(2, template.BehaviorData.Count);
            Assert.Equal(2, template.AbilityIds.Count);
        }

        #endregion

        #region Edge Cases and Error Conditions

        [Fact]
        public void EdgeCase_MaximumDataLoad()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("Load Test NPC", "Testing maximum data load", CreateDefaultStats(), new List<Guid>());

            // Act - Add large amounts of data
            for (int i = 0; i < 1000; i++)
            {
                template.AddConfiguration($"Config_{i}", $"Value_{i}");
                template.SetBehaviorData($"Behavior_{i}", i);
                template.AddAbility(Guid.NewGuid());
                template.AddLootItem(Guid.NewGuid());
            }

            // Assert
            Assert.Equal(1000, template.ConfigurationData.Count);
            Assert.Equal(1000, template.BehaviorData.Count);
            Assert.Equal(1000, template.AbilityIds.Count);
            Assert.Equal(1000, template.LootTableIds.Count);

            // Verify random access
            Assert.Equal("Value_500", template.GetConfiguration<string>("Config_500"));
            Assert.Equal(750, template.GetBehaviorData<int>("Behavior_750"));
        }

        [Fact]
        public void EdgeCase_TypeSafetyWithComplexObjects()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("Type Test", "Testing type safety", CreateDefaultStats(), new List<Guid>());
            var complexObject = new { Name = "Test", Value = 42, Items = new[] { "A", "B", "C" } };
            var dictionary = new Dictionary<string, int> { { "key1", 1 }, { "key2", 2 } };

            // Act
            template.AddConfiguration("ComplexObject", complexObject);
            template.AddConfiguration("Dictionary", dictionary);
            template.SetBehaviorData("ComplexBehavior", complexObject);

            // Assert - Correct type retrieval
            var retrievedComplex = template.GetConfiguration<dynamic>("ComplexObject");
            Assert.NotNull(retrievedComplex);
            
            var retrievedDict = template.GetConfiguration<Dictionary<string, int>>("Dictionary");
            Assert.Equal(2, retrievedDict.Count);
            Assert.Equal(1, retrievedDict["key1"]);

            // Wrong type retrieval should return defaults
            Assert.Null(template.GetConfiguration<string>("ComplexObject"));
            Assert.Null(template.GetConfiguration<List<string>>("Dictionary"));
        }

        [Fact]
        public void EdgeCase_ConfigurationKeyEdgeCases()
        {
            // Arrange
            var template = CharacterTemplate.CreateNPCTemplate("Edge Test", "Testing edge cases", CreateDefaultStats(), new List<Guid>());

            // Act & Assert - Test various key edge cases
            template.AddConfiguration("a", "single char key");
            template.AddConfiguration("Key with spaces", "spaces allowed");
            template.AddConfiguration("KEY_WITH_UNDERSCORES", "underscores allowed");
            template.AddConfiguration("Key123WithNumbers", "numbers allowed");
            template.AddConfiguration("SpecialChar!@#Key", "special chars allowed");
            template.AddConfiguration("VeryLongKeyNameThatIsQuiteLongButStillValid", "long keys work");

            Assert.Equal(6, template.ConfigurationData.Count);
            Assert.Equal("single char key", template.GetConfiguration<string>("a"));
            Assert.Equal("spaces allowed", template.GetConfiguration<string>("Key with spaces"));
        }

        #endregion

        #region Enum Value Tests

        [Theory]
        [InlineData(CharacterType.Player)]
        [InlineData(CharacterType.NPC)]
        public void Constructor_AllCharacterTypes_WorkCorrectly(CharacterType characterType)
        {
            // Arrange
            var name = "Test Character";
            var description = "Test Description";
            var baseStats = CreateDefaultStats();

            // Act
            var template = new CharacterTemplate(name, description, characterType, baseStats);

            // Assert
            Assert.Equal(characterType, template.CharacterType);
        }

        [Theory]
        [InlineData(PlayerClass.Warrior)]
        [InlineData(PlayerClass.Mage)]
        [InlineData(PlayerClass.Rogue)]
        [InlineData(PlayerClass.Archer)]
        [InlineData(PlayerClass.Paladin)]
        [InlineData(PlayerClass.Necromancer)]
        public void CreatePlayerTemplate_AllPlayerClasses_SetCorrectly(PlayerClass playerClass)
        {
            // Arrange
            var name = $"Test {playerClass}";
            var description = $"A {playerClass} character";
            var baseStats = CreateDefaultStats();
            var abilityIds = new List<Guid>();

            // Act
            var template = CharacterTemplate.CreatePlayerTemplate(name, description, baseStats, abilityIds, playerClass);

            // Assert
            Assert.Equal(playerClass, template.PlayerClass);
            Assert.Equal(CharacterType.Player, template.CharacterType);
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
        public void CreateNPCTemplate_AllNPCBehaviors_SetCorrectly(NPCBehavior npcBehavior)
        {
            // Arrange
            var name = $"Test {npcBehavior} NPC";
            var description = $"An NPC with {npcBehavior} behavior";
            var baseStats = CreateDefaultStats();
            var abilityIds = new List<Guid>();

            // Act
            var template = CharacterTemplate.CreateNPCTemplate(name, description, baseStats, abilityIds, npcBehavior);

            // Assert
            Assert.Equal(npcBehavior, template.NPCBehavior);
            Assert.Equal(CharacterType.NPC, template.CharacterType);
        }

        #endregion
    }
}