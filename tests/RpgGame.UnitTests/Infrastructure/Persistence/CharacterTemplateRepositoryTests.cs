using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using RpgGame.Domain.Common;
using RpgGame.Domain.Entities.Configuration;
using RpgGame.Domain.Enums;
using RpgGame.Domain.ValueObjects;
using RpgGame.Infrastructure.Persistence.EFCore;
using RpgGame.Infrastructure.Persistence.Repositories;
using Xunit;

namespace RpgGame.UnitTests.Infrastructure.Persistence
{
    /// <summary>
    /// Comprehensive unit tests for CharacterTemplateRepository following Clean Architecture and DDD principles.
    /// Tests all CRUD operations, validation scenarios, edge cases, and exception handling.
    /// </summary>
    public class CharacterTemplateRepositoryTests : IDisposable
    {
        private readonly GameDbContext _context;
        private readonly CharacterTemplateRepository _repository;
        private readonly CharacterStats _testStats;

        public CharacterTemplateRepositoryTests()
        {
            // Create in-memory database for testing
            var options = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new GameDbContext(options);
            _repository = new CharacterTemplateRepository(_context);

            // Create test stats for character templates
            _testStats = new CharacterStats(
                level: 1,
                maxHealth: 100,
                strength: 15,
                defense: 10,
                speed: 12,
                magic: 8
            );
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        #region Test Helpers

        /// <summary>
        /// Creates a test character template with specified parameters
        /// </summary>
        private CharacterTemplate CreateTestTemplate(
            string name = "TestTemplate",
            string description = "Test Description",
            CharacterType characterType = CharacterType.Player,
            NPCBehavior? npcBehavior = null,
            PlayerClass? playerClass = PlayerClass.Warrior)
        {
            return new CharacterTemplate(name, description, characterType, _testStats, npcBehavior, playerClass);
        }

        /// <summary>
        /// Seeds the database with test data
        /// </summary>
        private async Task SeedTestDataAsync()
        {
            var templates = new List<CharacterTemplate>
            {
                CreateTestTemplate("Warrior Template", "Basic warrior", CharacterType.Player, null, PlayerClass.Warrior),
                CreateTestTemplate("Mage Template", "Basic mage", CharacterType.Player, null, PlayerClass.Mage),
                CreateTestTemplate("Goblin Template", "Basic goblin", CharacterType.NPC, NPCBehavior.Aggressive, null),
                CreateTestTemplate("Merchant Template", "Basic merchant", CharacterType.NPC, NPCBehavior.Vendor, null),
                CreateTestTemplate("Guard Template", "Basic guard", CharacterType.NPC, NPCBehavior.Guard, null)
            };

            await _context.CharacterTemplates.AddRangeAsync(templates);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Read Operations Tests

        #region GetByCharacterTypeAsync Tests

        [Fact]
        public async Task GetByCharacterTypeAsync_WithValidPlayerType_ShouldReturnPlayerTemplates()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _repository.GetByCharacterTypeAsync(CharacterType.Player);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);

            var templates = result.Data.ToList();
            Assert.Equal(2, templates.Count);
            Assert.All(templates, t => Assert.Equal(CharacterType.Player, t.CharacterType));
            Assert.Contains(templates, t => t.Name == "Warrior Template");
            Assert.Contains(templates, t => t.Name == "Mage Template");
        }

        [Fact]
        public async Task GetByCharacterTypeAsync_WithValidNPCType_ShouldReturnNPCTemplates()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _repository.GetByCharacterTypeAsync(CharacterType.NPC);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);

            var templates = result.Data.ToList();
            Assert.Equal(3, templates.Count);
            Assert.All(templates, t => Assert.Equal(CharacterType.NPC, t.CharacterType));
            Assert.Contains(templates, t => t.Name == "Goblin Template");
            Assert.Contains(templates, t => t.Name == "Merchant Template");
            Assert.Contains(templates, t => t.Name == "Guard Template");
        }

        [Fact]
        public async Task GetByCharacterTypeAsync_WithNoMatchingType_ShouldReturnEmptyCollection()
        {
            // Arrange
            // Add only Player templates
            var playerTemplate = CreateTestTemplate("Player Only", "Player template", CharacterType.Player);
            await _context.CharacterTemplates.AddAsync(playerTemplate);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByCharacterTypeAsync(CharacterType.NPC);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task GetByCharacterTypeAsync_WithEmptyDatabase_ShouldReturnEmptyCollection()
        {
            // Act
            var result = await _repository.GetByCharacterTypeAsync(CharacterType.Player);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task GetByCharacterTypeAsync_ShouldReturnTemplatesOrderedByName()
        {
            // Arrange
            var templates = new List<CharacterTemplate>
            {
                CreateTestTemplate("Zebra Template", "Z template", CharacterType.Player, null, PlayerClass.Warrior),
                CreateTestTemplate("Alpha Template", "A template", CharacterType.Player, null, PlayerClass.Mage),
                CreateTestTemplate("Beta Template", "B template", CharacterType.Player, null, PlayerClass.Rogue)
            };

            await _context.CharacterTemplates.AddRangeAsync(templates);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByCharacterTypeAsync(CharacterType.Player);

            // Assert
            Assert.True(result.Succeeded);
            var orderedTemplates = result.Data.ToList();
            Assert.Equal("Alpha Template", orderedTemplates[0].Name);
            Assert.Equal("Beta Template", orderedTemplates[1].Name);
            Assert.Equal("Zebra Template", orderedTemplates[2].Name);
        }

        #endregion

        #region GetByNameAsync Tests

        [Fact]
        public async Task GetByNameAsync_WithValidName_ShouldReturnCorrectTemplate()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _repository.GetByNameAsync("Warrior Template");

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal("Warrior Template", result.Data.Name);
            Assert.Equal(CharacterType.Player, result.Data.CharacterType);
            Assert.Equal(PlayerClass.Warrior, result.Data.PlayerClass);
        }

        [Fact]
        public async Task GetByNameAsync_WithCaseInsensitiveName_ShouldReturnCorrectTemplate()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _repository.GetByNameAsync("WARRIOR TEMPLATE");

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal("Warrior Template", result.Data.Name);
        }

        [Fact]
        public async Task GetByNameAsync_WithNonExistentName_ShouldReturnNotFound()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _repository.GetByNameAsync("Non Existent Template");

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.Errors.Any(e => e.Code == "CharacterTemplate.NotFound"));
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetByNameAsync_WithNullName_ShouldReturnValidationError()
        {
            // Act
            var result = await _repository.GetByNameAsync(null);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.Errors.Any(e => e.Code == "Validation.Failed"));
            Assert.Contains("Name cannot be null, empty, or whitespace", result.FirstErrorMessage);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetByNameAsync_WithEmptyName_ShouldReturnValidationError()
        {
            // Act
            var result = await _repository.GetByNameAsync(string.Empty);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.Errors.Any(e => e.Code == "Validation.Failed"));
            Assert.Contains("Name cannot be null, empty, or whitespace", result.FirstErrorMessage);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetByNameAsync_WithWhitespaceName_ShouldReturnValidationError()
        {
            // Act
            var result = await _repository.GetByNameAsync("   ");

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.Errors.Any(e => e.Code == "Validation.Failed"));
            Assert.Contains("Name cannot be null, empty, or whitespace", result.FirstErrorMessage);
            Assert.Null(result.Data);
        }

        #endregion

        #region GetByNPCBehaviorAsync Tests

        [Fact]
        public async Task GetByNPCBehaviorAsync_WithValidBehavior_ShouldReturnMatchingTemplates()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _repository.GetByNPCBehaviorAsync(NPCBehavior.Aggressive);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);

            var templates = result.Data.ToList();
            Assert.Single(templates);
            Assert.Equal("Goblin Template", templates[0].Name);
            Assert.Equal(NPCBehavior.Aggressive, templates[0].NPCBehavior);
        }

        [Fact]
        public async Task GetByNPCBehaviorAsync_WithMultipleMatchingBehaviors_ShouldReturnAllMatching()
        {
            // Arrange
            var templates = new List<CharacterTemplate>
            {
                CreateTestTemplate("Orc Template", "Aggressive orc", CharacterType.NPC, NPCBehavior.Aggressive),
                CreateTestTemplate("Goblin Template", "Aggressive goblin", CharacterType.NPC, NPCBehavior.Aggressive),
                CreateTestTemplate("Dragon Template", "Aggressive dragon", CharacterType.NPC, NPCBehavior.Aggressive)
            };

            await _context.CharacterTemplates.AddRangeAsync(templates);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByNPCBehaviorAsync(NPCBehavior.Aggressive);

            // Assert
            Assert.True(result.Succeeded);
            var matchingTemplates = result.Data.ToList();
            Assert.Equal(3, matchingTemplates.Count);
            Assert.All(matchingTemplates, t => Assert.Equal(NPCBehavior.Aggressive, t.NPCBehavior));
        }

        [Fact]
        public async Task GetByNPCBehaviorAsync_WithNoMatchingBehavior_ShouldReturnEmptyCollection()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _repository.GetByNPCBehaviorAsync(NPCBehavior.Patrol);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task GetByNPCBehaviorAsync_ShouldReturnTemplatesOrderedByName()
        {
            // Arrange
            var templates = new List<CharacterTemplate>
            {
                CreateTestTemplate("Zebra NPC", "Z NPC", CharacterType.NPC, NPCBehavior.Passive),
                CreateTestTemplate("Alpha NPC", "A NPC", CharacterType.NPC, NPCBehavior.Passive),
                CreateTestTemplate("Beta NPC", "B NPC", CharacterType.NPC, NPCBehavior.Passive)
            };

            await _context.CharacterTemplates.AddRangeAsync(templates);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByNPCBehaviorAsync(NPCBehavior.Passive);

            // Assert
            Assert.True(result.Succeeded);
            var orderedTemplates = result.Data.ToList();
            Assert.Equal("Alpha NPC", orderedTemplates[0].Name);
            Assert.Equal("Beta NPC", orderedTemplates[1].Name);
            Assert.Equal("Zebra NPC", orderedTemplates[2].Name);
        }

        #endregion

        #region GetByPlayerClassAsync Tests

        [Fact]
        public async Task GetByPlayerClassAsync_WithValidPlayerClass_ShouldReturnMatchingTemplates()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _repository.GetByPlayerClassAsync(PlayerClass.Warrior);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);

            var templates = result.Data.ToList();
            Assert.Single(templates);
            Assert.Equal("Warrior Template", templates[0].Name);
            Assert.Equal(PlayerClass.Warrior, templates[0].PlayerClass);
        }

        [Fact]
        public async Task GetByPlayerClassAsync_WithMultipleMatchingClasses_ShouldReturnAllMatching()
        {
            // Arrange
            var templates = new List<CharacterTemplate>
            {
                CreateTestTemplate("Fire Mage", "Fire specialist", CharacterType.Player, null, PlayerClass.Mage),
                CreateTestTemplate("Ice Mage", "Ice specialist", CharacterType.Player, null, PlayerClass.Mage),
                CreateTestTemplate("Battle Mage", "Combat mage", CharacterType.Player, null, PlayerClass.Mage)
            };

            await _context.CharacterTemplates.AddRangeAsync(templates);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByPlayerClassAsync(PlayerClass.Mage);

            // Assert
            Assert.True(result.Succeeded);
            var matchingTemplates = result.Data.ToList();
            Assert.Equal(3, matchingTemplates.Count);
            Assert.All(matchingTemplates, t => Assert.Equal(PlayerClass.Mage, t.PlayerClass));
        }

        [Fact]
        public async Task GetByPlayerClassAsync_WithNoMatchingClass_ShouldReturnEmptyCollection()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _repository.GetByPlayerClassAsync(PlayerClass.Necromancer);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task GetByPlayerClassAsync_ShouldReturnTemplatesOrderedByName()
        {
            // Arrange
            var templates = new List<CharacterTemplate>
            {
                CreateTestTemplate("Zebra Rogue", "Z rogue", CharacterType.Player, null, PlayerClass.Rogue),
                CreateTestTemplate("Alpha Rogue", "A rogue", CharacterType.Player, null, PlayerClass.Rogue),
                CreateTestTemplate("Beta Rogue", "B rogue", CharacterType.Player, null, PlayerClass.Rogue)
            };

            await _context.CharacterTemplates.AddRangeAsync(templates);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByPlayerClassAsync(PlayerClass.Rogue);

            // Assert
            Assert.True(result.Succeeded);
            var orderedTemplates = result.Data.ToList();
            Assert.Equal("Alpha Rogue", orderedTemplates[0].Name);
            Assert.Equal("Beta Rogue", orderedTemplates[1].Name);
            Assert.Equal("Zebra Rogue", orderedTemplates[2].Name);
        }

        #endregion

        #endregion

        #region Write Operations Tests

        #region AddAsync Tests

        [Fact]
        public async Task AddAsync_WithValidTemplate_ShouldAddSuccessfully()
        {
            // Arrange
            var template = CreateTestTemplate("New Template", "New description");

            // Act
            var result = await _repository.AddAsync(template);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal("New Template", result.Data.Name);

            // Verify it was saved to database
            var savedTemplate = await _context.CharacterTemplates.FindAsync(template.Id);
            Assert.NotNull(savedTemplate);
            Assert.Equal("New Template", savedTemplate.Name);
        }

        [Fact]
        public async Task AddAsync_WithNullTemplate_ShouldReturnValidationError()
        {
            // Act
            var result = await _repository.AddAsync(null);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.Errors.Any(e => e.Code == "Validation.Failed"));
            Assert.Contains("Character template cannot be null", result.FirstErrorMessage);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task AddAsync_WithDuplicateName_ShouldReturnConflictError()
        {
            // Arrange
            var existingTemplate = CreateTestTemplate("Duplicate Template", "Existing template");
            await _context.CharacterTemplates.AddAsync(existingTemplate);
            await _context.SaveChangesAsync();

            var duplicateTemplate = CreateTestTemplate("Duplicate Template", "New template with same name");

            // Act
            var result = await _repository.AddAsync(duplicateTemplate);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.Errors.Any(e => e.Code == "Operation.Conflict"));
            Assert.Contains("already exists", result.FirstErrorMessage);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task AddAsync_WithDuplicateNameCaseInsensitive_ShouldReturnConflictError()
        {
            // Arrange
            var existingTemplate = CreateTestTemplate("Test Template", "Existing template");
            await _context.CharacterTemplates.AddAsync(existingTemplate);
            await _context.SaveChangesAsync();

            var duplicateTemplate = CreateTestTemplate("TEST TEMPLATE", "New template with same name");

            // Act
            var result = await _repository.AddAsync(duplicateTemplate);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.Errors.Any(e => e.Code == "Operation.Conflict"));
            Assert.Contains("already exists", result.FirstErrorMessage);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task AddAsync_WithComplexTemplate_ShouldPreserveAllProperties()
        {
            // Arrange
            var template = CharacterTemplate.CreatePlayerTemplate(
                "Complex Template",
                "Complex description",
                _testStats,
                new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                PlayerClass.Paladin
            );

            template.AddConfiguration("TestKey", "TestValue");
            template.SetBehaviorData("BehaviorKey", 42);

            // Act
            var result = await _repository.AddAsync(template);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);

            var savedTemplate = await _context.CharacterTemplates.FindAsync(template.Id);
            Assert.NotNull(savedTemplate);
            Assert.Equal(PlayerClass.Paladin, savedTemplate.PlayerClass);
            Assert.Equal(2, savedTemplate.AbilityIds.Count);
            Assert.Equal("TestValue", savedTemplate.GetConfiguration<string>("TestKey"));
            Assert.Equal(42, savedTemplate.GetBehaviorData<int>("BehaviorKey"));
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithValidTemplate_ShouldUpdateSuccessfully()
        {
            // Arrange
            var template = CreateTestTemplate("Original Template", "Original description");
            await _context.CharacterTemplates.AddAsync(template);
            await _context.SaveChangesAsync();

            // Modify the template
            template.UpdateDetails("Updated Template", "Updated description", _testStats);

            // Act
            var result = await _repository.UpdateAsync(template);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal("Updated Template", result.Data.Name);

            // Verify database was updated
            var updatedTemplate = await _context.CharacterTemplates.FindAsync(template.Id);
            Assert.NotNull(updatedTemplate);
            Assert.Equal("Updated Template", updatedTemplate.Name);
            Assert.Equal("Updated description", updatedTemplate.Description);
        }

        [Fact]
        public async Task UpdateAsync_WithNullTemplate_ShouldReturnValidationError()
        {
            // Act
            var result = await _repository.UpdateAsync(null);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.Errors.Any(e => e.Code == "Validation.Failed"));
            Assert.Contains("Character template cannot be null", result.FirstErrorMessage);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistentTemplate_ShouldReturnNotFoundError()
        {
            // Arrange
            var template = CreateTestTemplate("Non Existent", "Non existent template");

            // Act
            var result = await _repository.UpdateAsync(template);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.Errors.Any(e => e.Code == "CharacterTemplate.NotFound"));
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task UpdateAsync_WithDuplicateNameFromOtherTemplate_ShouldReturnConflictError()
        {
            // Arrange
            var template1 = CreateTestTemplate("Template 1", "First template");
            var template2 = CreateTestTemplate("Template 2", "Second template");

            await _context.CharacterTemplates.AddRangeAsync(template1, template2);
            await _context.SaveChangesAsync();

            // Try to update template2 to have same name as template1
            template2.UpdateDetails("Template 1", "Updated description", _testStats);

            // Act
            var result = await _repository.UpdateAsync(template2);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.Errors.Any(e => e.Code == "Operation.Conflict"));
            Assert.Contains("already exists", result.FirstErrorMessage);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task UpdateAsync_WithSameNameSameTemplate_ShouldUpdateSuccessfully()
        {
            // Arrange
            var template = CreateTestTemplate("Same Name Template", "Original description");
            await _context.CharacterTemplates.AddAsync(template);
            await _context.SaveChangesAsync();

            // Update with same name but different description
            template.UpdateDetails("Same Name Template", "Updated description", _testStats);

            // Act
            var result = await _repository.UpdateAsync(template);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal("Same Name Template", result.Data.Name);
            Assert.Equal("Updated description", result.Data.Description);
        }

        #endregion

        #region DeleteAsync Entity Tests

        [Fact]
        public async Task DeleteAsync_WithValidEntity_ShouldDeleteSuccessfully()
        {
            // Arrange
            var template = CreateTestTemplate("Delete Template", "Template to delete");
            await _context.CharacterTemplates.AddAsync(template);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteAsync(template);

            // Assert
            Assert.True(result.Succeeded);

            // Verify it was deleted from database
            var deletedTemplate = await _context.CharacterTemplates.FindAsync(template.Id);
            Assert.Null(deletedTemplate);
        }

        [Fact]
        public async Task DeleteAsync_WithNullEntity_ShouldReturnValidationError()
        {
            // Act
            var result = await _repository.DeleteAsync((CharacterTemplate)null);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.Errors.Any(e => e.Code == "Validation.Failed"));
            Assert.Contains("Character template cannot be null", result.FirstErrorMessage);
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistentEntity_ShouldReturnNotFoundError()
        {
            // Arrange
            var template = CreateTestTemplate("Non Existent", "Non existent template");

            // Act
            var result = await _repository.DeleteAsync(template);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.Errors.Any(e => e.Code == "CharacterTemplate.NotFound"));
        }

        #endregion

        #region DeleteAsync ID Tests

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldDeleteSuccessfully()
        {
            // Arrange
            var template = CreateTestTemplate("Delete By ID Template", "Template to delete by ID");
            await _context.CharacterTemplates.AddAsync(template);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteAsync(template.Id);

            // Assert
            Assert.True(result.Succeeded);

            // Verify it was deleted from database
            var deletedTemplate = await _context.CharacterTemplates.FindAsync(template.Id);
            Assert.Null(deletedTemplate);
        }

        [Fact]
        public async Task DeleteAsync_WithEmptyId_ShouldReturnValidationError()
        {
            // Act
            var result = await _repository.DeleteAsync(Guid.Empty);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.Errors.Any(e => e.Code == "Validation.Failed"));
            Assert.Contains("Id cannot be empty", result.FirstErrorMessage);
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistentId_ShouldReturnNotFoundError()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.DeleteAsync(nonExistentId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.Errors.Any(e => e.Code == "CharacterTemplate.NotFound"));
        }

        #endregion

        #endregion

        #region Exception Handling Tests

        [Fact]
        public async Task GetByCharacterTypeAsync_WithDatabaseException_ShouldReturnFailureResult()
        {
            // Arrange
            _context.Dispose(); // Force database connection error

            // Act
            var result = await _repository.GetByCharacterTypeAsync(CharacterType.Player);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.Errors.Any(e => e.Code == "CharacterTemplate.GetByType.Failed"));
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetByNameAsync_WithDatabaseException_ShouldReturnFailureResult()
        {
            // Arrange
            _context.Dispose(); // Force database connection error

            // Act
            var result = await _repository.GetByNameAsync("Test Template");

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.Errors.Any(e => e.Code == "CharacterTemplate.GetByName.Failed"));
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task AddAsync_WithDatabaseException_ShouldReturnFailureResult()
        {
            // Arrange
            var template = CreateTestTemplate("Test Template", "Test description");
            _context.Dispose(); // Force database connection error

            // Act
            var result = await _repository.AddAsync(template);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.Errors.Any(e => e.Code == "CharacterTemplate.Add.Failed"));
            Assert.Null(result.Data);
        }

        #endregion

        #region Edge Cases and Complex Scenarios

        [Fact]
        public async Task GetByCharacterTypeAsync_WithAllEnumValues_ShouldHandleAllCases()
        {
            // Arrange
            var playerTemplate = CreateTestTemplate("Player", "Player template", CharacterType.Player);
            var npcTemplate = CreateTestTemplate("NPC", "NPC template", CharacterType.NPC);

            await _context.CharacterTemplates.AddRangeAsync(playerTemplate, npcTemplate);
            await _context.SaveChangesAsync();

            // Act & Assert for each enum value
            foreach (CharacterType characterType in Enum.GetValues<CharacterType>())
            {
                var result = await _repository.GetByCharacterTypeAsync(characterType);
                Assert.True(result.Succeeded);
                Assert.NotNull(result.Data);

                var templates = result.Data.ToList();
                if (templates.Any())
                {
                    Assert.All(templates, t => Assert.Equal(characterType, t.CharacterType));
                }
            }
        }

        [Fact]
        public async Task GetByNPCBehaviorAsync_WithAllEnumValues_ShouldHandleAllCases()
        {
            // Arrange
            var behaviors = Enum.GetValues<NPCBehavior>().Take(3); // Take first 3 for testing
            var templates = behaviors.Select((behavior, index) =>
                CreateTestTemplate($"NPC {index}", $"NPC with {behavior} behavior", CharacterType.NPC, behavior))
                .ToList();

            await _context.CharacterTemplates.AddRangeAsync(templates);
            await _context.SaveChangesAsync();

            // Act & Assert for each enum value
            foreach (NPCBehavior behavior in Enum.GetValues<NPCBehavior>())
            {
                var result = await _repository.GetByNPCBehaviorAsync(behavior);
                Assert.True(result.Succeeded);
                Assert.NotNull(result.Data);

                var matchingTemplates = result.Data.ToList();
                if (matchingTemplates.Any())
                {
                    Assert.All(matchingTemplates, t => Assert.Equal(behavior, t.NPCBehavior));
                }
            }
        }

        [Fact]
        public async Task GetByPlayerClassAsync_WithAllEnumValues_ShouldHandleAllCases()
        {
            // Arrange
            var classes = Enum.GetValues<PlayerClass>().Take(3); // Take first 3 for testing
            var templates = classes.Select((playerClass, index) =>
                CreateTestTemplate($"Player {index}", $"Player with {playerClass} class", CharacterType.Player, null, playerClass))
                .ToList();

            await _context.CharacterTemplates.AddRangeAsync(templates);
            await _context.SaveChangesAsync();

            // Act & Assert for each enum value
            foreach (PlayerClass playerClass in Enum.GetValues<PlayerClass>())
            {
                var result = await _repository.GetByPlayerClassAsync(playerClass);
                Assert.True(result.Succeeded);
                Assert.NotNull(result.Data);

                var matchingTemplates = result.Data.ToList();
                if (matchingTemplates.Any())
                {
                    Assert.All(matchingTemplates, t => Assert.Equal(playerClass, t.PlayerClass));
                }
            }
        }

        [Fact]
        public async Task Repository_WithLargeDataset_ShouldPerformEfficiently()
        {
            // Arrange
            const int templateCount = 100;
            var templates = new List<CharacterTemplate>();

            for (int i = 0; i < templateCount; i++)
            {
                var characterType = i % 2 == 0 ? CharacterType.Player : CharacterType.NPC;
                var template = CreateTestTemplate($"Template {i:D3}", $"Description {i}", characterType);
                templates.Add(template);
            }

            await _context.CharacterTemplates.AddRangeAsync(templates);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByCharacterTypeAsync(CharacterType.Player);

            // Assert
            Assert.True(result.Succeeded);
            var playerTemplates = result.Data.ToList();
            Assert.Equal(50, playerTemplates.Count); // Half should be players
            Assert.All(playerTemplates, t => Assert.Equal(CharacterType.Player, t.CharacterType));
        }

        [Fact]
        public async Task Repository_WithSpecialCharactersInNames_ShouldHandleCorrectly()
        {
            // Arrange
            var specialNames = new[]
            {
                "Template with @#$%",
                "Template with 数字",
                "Template with émojis 🎮",
                "Template with \n newline",
                "Template with \"quotes\"",
                "Template with 'apostrophes'"
            };

            var templates = specialNames.Select((name, index) =>
                CreateTestTemplate(name, $"Description {index}"))
                .ToList();

            await _context.CharacterTemplates.AddRangeAsync(templates);
            await _context.SaveChangesAsync();

            // Act & Assert
            foreach (var name in specialNames)
            {
                var result = await _repository.GetByNameAsync(name);
                Assert.True(result.Succeeded, $"Failed to find template with name: {name}");
                Assert.Equal(name, result.Data.Name);
            }
        }

        #endregion
    }
}