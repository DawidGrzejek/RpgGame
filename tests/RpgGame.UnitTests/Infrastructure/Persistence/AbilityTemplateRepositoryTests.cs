using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RpgGame.Domain.Common;
using RpgGame.Domain.Entities.Configuration;
using RpgGame.Domain.Enums;
using RpgGame.Infrastructure.Persistence.EFCore;
using RpgGame.Infrastructure.Persistence.Repositories;
using Xunit;

namespace RpgGame.UnitTests.Infrastructure.Persistence
{
    /// <summary>
    /// Comprehensive unit tests for AbilityTemplateRepository following Clean Architecture and DDD principles.
    /// Tests all CRUD operations, validation scenarios, edge cases, and exception handling.
    /// </summary>
    public class AbilityTemplateRepositoryTests : IDisposable
    {
        private readonly GameDbContext _context;
        private readonly AbilityTemplateRepository _repository;

        public AbilityTemplateRepositoryTests()
        {
            // Create in-memory database for testing
            var options = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new GameDbContext(options);
            _repository = new AbilityTemplateRepository(_context);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        #region Test Helpers

        /// <summary>
        /// Creates a test ability template with specified parameters
        /// </summary>
        private AbilityTemplate CreateTestTemplate(
            string name = "TestAbility",
            string description = "Test Description",
            AbilityType abilityType = AbilityType.Active,
            TargetType targetType = TargetType.SingleEnemy,
            int manaCost = 10,
            int cooldown = 5,
            int range = 1)
        {
            var template = new AbilityTemplate(name, description, abilityType, targetType, manaCost, cooldown, range);
            return template;
        }

        /// <summary>
        /// Creates a test ability template with effects
        /// </summary>
        private AbilityTemplate CreateTestTemplateWithEffects(
            string name = "TestAbilityWithEffects",
            string description = "Test ability with effects")
        {
            var template = CreateTestTemplate(name, description);

            // Add some test effects
            var damageEffect = new AbilityEffect(EffectType.PhysicalDamage, 50, 0);
            damageEffect.SetParameter("criticalChance", 0.1);
            template.AddEffect(damageEffect);

            var healEffect = new AbilityEffect(EffectType.InstantHeal, 25, 0);
            healEffect.SetParameter("overHealProtection", true);
            template.AddEffect(healEffect);

            // Set visuals and requirements
            template.SetVisuals("fire_explosion", "explosion_sound");
            template.AddRequirement("Level", 5);
            template.AddRequirement("Class", "Mage");

            return template;
        }

        /// <summary>
        /// Seeds the database with test data
        /// </summary>
        private async Task SeedTestDataAsync()
        {
            var templates = new List<AbilityTemplate>
            {
                CreateTestTemplate("Fireball", "Basic fire spell", AbilityType.Active, TargetType.SingleEnemy, 15, 3, 3),
                CreateTestTemplate("Heal", "Basic healing spell", AbilityType.Active, TargetType.SingleAlly, 10, 2, 2),
                CreateTestTemplate("Stealth", "Passive stealth ability", AbilityType.Passive, TargetType.Self, 0, 0, 0),
                CreateTestTemplate("Lightning Bolt", "Area lightning attack", AbilityType.Channeled, TargetType.Area, 25, 5, 4),
                CreateTestTemplate("Shield Wall", "Toggle defensive ability", AbilityType.Toggle, TargetType.None, 5, 1, 0)
            };

            await _context.AbilityTemplates.AddRangeAsync(templates);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Read Operations Tests

        #region GetByAbilityTypeAsync Tests

        /// <summary>
        /// Tests successful retrieval of abilities by Active type
        /// </summary>
        [Fact]
        public async Task GetByAbilityTypeAsync_WithValidActiveType_ShouldReturnActiveAbilities()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _repository.GetByAbilityTypeAsync(AbilityType.Active);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);

            var abilities = result.Data.ToList();
            Assert.Equal(2, abilities.Count);
            Assert.All(abilities, a => Assert.Equal(AbilityType.Active, a.AbilityType));
            Assert.Contains(abilities, a => a.Name == "Fireball");
            Assert.Contains(abilities, a => a.Name == "Heal");
        }

        /// <summary>
        /// Tests successful retrieval of abilities by Passive type
        /// </summary>
        [Fact]
        public async Task GetByAbilityTypeAsync_WithValidPassiveType_ShouldReturnPassiveAbilities()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _repository.GetByAbilityTypeAsync(AbilityType.Passive);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);

            var abilities = result.Data.ToList();
            Assert.Single(abilities);
            Assert.Equal(AbilityType.Passive, abilities[0].AbilityType);
            Assert.Equal("Stealth", abilities[0].Name);
        }

        /// <summary>
        /// Tests successful retrieval of abilities by Toggle type
        /// </summary>
        [Fact]
        public async Task GetByAbilityTypeAsync_WithValidToggleType_ShouldReturnToggleAbilities()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _repository.GetByAbilityTypeAsync(AbilityType.Toggle);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);

            var abilities = result.Data.ToList();
            Assert.Single(abilities);
            Assert.Equal(AbilityType.Toggle, abilities[0].AbilityType);
            Assert.Equal("Shield Wall", abilities[0].Name);
        }

        /// <summary>
        /// Tests successful retrieval of abilities by Channeled type
        /// </summary>
        [Fact]
        public async Task GetByAbilityTypeAsync_WithValidChanneledType_ShouldReturnChanneledAbilities()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _repository.GetByAbilityTypeAsync(AbilityType.Channeled);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);

            var abilities = result.Data.ToList();
            Assert.Single(abilities);
            Assert.Equal(AbilityType.Channeled, abilities[0].AbilityType);
            Assert.Equal("Lightning Bolt", abilities[0].Name);
        }

        /// <summary>
        /// Tests retrieval with no matching ability type returns empty collection
        /// </summary>
        [Fact]
        public async Task GetByAbilityTypeAsync_WithNoMatchingType_ShouldReturnEmptyCollection()
        {
            // Arrange
            // Add only Active abilities
            var activeAbility = CreateTestTemplate("Active Only", "Active ability", AbilityType.Active);
            await _context.AbilityTemplates.AddAsync(activeAbility);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByAbilityTypeAsync(AbilityType.Passive);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
        }

        /// <summary>
        /// Tests retrieval from empty database returns empty collection
        /// </summary>
        [Fact]
        public async Task GetByAbilityTypeAsync_WithEmptyDatabase_ShouldReturnEmptyCollection()
        {
            // Act
            var result = await _repository.GetByAbilityTypeAsync(AbilityType.Active);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
        }

        #endregion

        #region GetByTargetTypeAsync Tests

        /// <summary>
        /// Tests successful retrieval of abilities by SingleEnemy target type
        /// </summary>
        [Fact]
        public async Task GetByTargetTypeAsync_WithValidSingleEnemyType_ShouldReturnMatchingAbilities()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _repository.GetByTargetTypeAsync(TargetType.SingleEnemy);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);

            var abilities = result.Data.ToList();
            Assert.Single(abilities);
            Assert.Equal(TargetType.SingleEnemy, abilities[0].TargetType);
            Assert.Equal("Fireball", abilities[0].Name);
        }

        /// <summary>
        /// Tests successful retrieval of abilities by SingleAlly target type
        /// </summary>
        [Fact]
        public async Task GetByTargetTypeAsync_WithValidSingleAllyType_ShouldReturnMatchingAbilities()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _repository.GetByTargetTypeAsync(TargetType.SingleAlly);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);

            var abilities = result.Data.ToList();
            Assert.Single(abilities);
            Assert.Equal(TargetType.SingleAlly, abilities[0].TargetType);
            Assert.Equal("Heal", abilities[0].Name);
        }

        /// <summary>
        /// Tests successful retrieval of abilities by Self target type
        /// </summary>
        [Fact]
        public async Task GetByTargetTypeAsync_WithValidSelfType_ShouldReturnMatchingAbilities()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _repository.GetByTargetTypeAsync(TargetType.Self);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);

            var abilities = result.Data.ToList();
            Assert.Single(abilities);
            Assert.Equal(TargetType.Self, abilities[0].TargetType);
            Assert.Equal("Stealth", abilities[0].Name);
        }

        /// <summary>
        /// Tests successful retrieval of abilities by Area target type
        /// </summary>
        [Fact]
        public async Task GetByTargetTypeAsync_WithValidAreaType_ShouldReturnMatchingAbilities()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _repository.GetByTargetTypeAsync(TargetType.Area);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);

            var abilities = result.Data.ToList();
            Assert.Single(abilities);
            Assert.Equal(TargetType.Area, abilities[0].TargetType);
            Assert.Equal("Lightning Bolt", abilities[0].Name);
        }

        /// <summary>
        /// Tests successful retrieval of abilities by None target type
        /// </summary>
        [Fact]
        public async Task GetByTargetTypeAsync_WithValidNoneType_ShouldReturnMatchingAbilities()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _repository.GetByTargetTypeAsync(TargetType.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);

            var abilities = result.Data.ToList();
            Assert.Single(abilities);
            Assert.Equal(TargetType.None, abilities[0].TargetType);
            Assert.Equal("Shield Wall", abilities[0].Name);
        }

        /// <summary>
        /// Tests retrieval with multiple abilities having same target type
        /// </summary>
        [Fact]
        public async Task GetByTargetTypeAsync_WithMultipleMatchingTargets_ShouldReturnAllMatching()
        {
            // Arrange
            var abilities = new List<AbilityTemplate>
            {
                CreateTestTemplate("Fireball", "Fire damage", AbilityType.Active, TargetType.SingleEnemy),
                CreateTestTemplate("Ice Shard", "Ice damage", AbilityType.Active, TargetType.SingleEnemy),
                CreateTestTemplate("Lightning Strike", "Lightning damage", AbilityType.Active, TargetType.SingleEnemy)
            };

            await _context.AbilityTemplates.AddRangeAsync(abilities);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByTargetTypeAsync(TargetType.SingleEnemy);

            // Assert
            Assert.True(result.Succeeded);
            var matchingAbilities = result.Data.ToList();
            Assert.Equal(3, matchingAbilities.Count);
            Assert.All(matchingAbilities, a => Assert.Equal(TargetType.SingleEnemy, a.TargetType));
        }

        /// <summary>
        /// Tests retrieval with no matching target type returns empty collection
        /// </summary>
        [Fact]
        public async Task GetByTargetTypeAsync_WithNoMatchingTargetType_ShouldReturnEmptyCollection()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _repository.GetByTargetTypeAsync(TargetType.AllEnemies);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
        }

        #endregion

        #region GetByNameAsync Tests

        /// <summary>
        /// Tests successful retrieval of ability by valid name
        /// </summary>
        [Fact]
        public async Task GetByNameAsync_WithValidName_ShouldReturnCorrectAbility()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _repository.GetByNameAsync("Fireball");

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal("Fireball", result.Data.Name);
            Assert.Equal(AbilityType.Active, result.Data.AbilityType);
            Assert.Equal(TargetType.SingleEnemy, result.Data.TargetType);
            Assert.Equal(15, result.Data.ManaCost);
        }

        /// <summary>
        /// Tests case insensitive name matching
        /// </summary>
        [Fact]
        public async Task GetByNameAsync_WithCaseInsensitiveName_ShouldReturnCorrectAbility()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _repository.GetByNameAsync("FIREBALL");

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal("Fireball", result.Data.Name);
        }

        /// <summary>
        /// Tests retrieval with non-existent name returns not found error
        /// </summary>
        [Fact]
        public async Task GetByNameAsync_WithNonExistentName_ShouldReturnNotFound()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var result = await _repository.GetByNameAsync("Non Existent Ability");

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("Ability not found", result.FirstErrorMessage);
            Assert.Null(result.Data);
        }

        /// <summary>
        /// Tests validation error with null name
        /// </summary>
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

        /// <summary>
        /// Tests validation error with empty name
        /// </summary>
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

        /// <summary>
        /// Tests validation error with whitespace name
        /// </summary>
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

        #region GetAvailableForCharacterAsync Tests

        /// <summary>
        /// Tests retrieval of abilities available for character with valid data
        /// </summary>
        [Fact]
        public async Task GetAvailableForCharacterAsync_WithValidCharacterData_ShouldReturnMatchingAbilities()
        {
            // Arrange
            var ability1 = CreateTestTemplate("Warrior Ability", "Warrior only", AbilityType.Active, TargetType.SingleEnemy);
            var ability2 = CreateTestTemplate("Mage Ability", "Mage only", AbilityType.Active, TargetType.Area);

            await _context.AbilityTemplates.AddRangeAsync(ability1, ability2);
            await _context.SaveChangesAsync();

            var characterData = new Dictionary<string, object>
            {
                { "AbilityType", AbilityType.Active }
            };

            // Act
            var result = await _repository.GetAvailableForCharacterAsync(characterData);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            var abilities = result.Data.ToList();
            Assert.Equal(2, abilities.Count);
            Assert.All(abilities, a => Assert.Equal(AbilityType.Active, a.AbilityType));
        }

        /// <summary>
        /// Tests retrieval with specific target type character data
        /// </summary>
        [Fact]
        public async Task GetAvailableForCharacterAsync_WithTargetTypeFilter_ShouldReturnMatchingAbilities()
        {
            // Arrange
            await SeedTestDataAsync();

            var characterData = new Dictionary<string, object>
            {
                { "TargetType", TargetType.SingleEnemy }
            };

            // Act
            var result = await _repository.GetAvailableForCharacterAsync(characterData);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            var abilities = result.Data.ToList();
            Assert.Single(abilities);
            Assert.Equal("Fireball", abilities[0].Name);
        }

        /// <summary>
        /// Tests retrieval with multiple criteria in character data
        /// </summary>
        [Fact]
        public async Task GetAvailableForCharacterAsync_WithMultipleCriteria_ShouldReturnMatchingAbilities()
        {
            // Arrange
            await SeedTestDataAsync();

            var characterData = new Dictionary<string, object>
            {
                { "AbilityType", AbilityType.Active },
                { "TargetType", TargetType.SingleAlly }
            };

            // Act
            var result = await _repository.GetAvailableForCharacterAsync(characterData);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            var abilities = result.Data.ToList();
            Assert.Single(abilities);
            Assert.Equal("Heal", abilities[0].Name);
        }

        /// <summary>
        /// Tests retrieval with null character data returns empty collection
        /// </summary>
        [Fact]
        public async Task GetAvailableForCharacterAsync_WithNullCharacterData_ShouldReturnEmptyCollection()
        {
            // Act
            var result = await _repository.GetAvailableForCharacterAsync(null);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
        }

        /// <summary>
        /// Tests retrieval with empty character data returns empty collection
        /// </summary>
        [Fact]
        public async Task GetAvailableForCharacterAsync_WithEmptyCharacterData_ShouldReturnEmptyCollection()
        {
            // Act
            var result = await _repository.GetAvailableForCharacterAsync(new Dictionary<string, object>());

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
        }

        #endregion

        #endregion

        #region Write Operations Tests

        #region AddAsync Tests

        /// <summary>
        /// Tests successful addition of valid ability template
        /// </summary>
        [Fact]
        public async Task AddAsync_WithValidTemplate_ShouldAddSuccessfully()
        {
            // Arrange
            var template = CreateTestTemplate("New Ability", "New ability description");

            // Act
            var result = await _repository.AddAsync(template);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal("New Ability", result.Data.Name);

            // Verify it was saved to database
            var savedTemplate = await _context.AbilityTemplates.FindAsync(template.Id);
            Assert.NotNull(savedTemplate);
            Assert.Equal("New Ability", savedTemplate.Name);
        }

        /// <summary>
        /// Tests validation error when adding null template
        /// </summary>
        [Fact]
        public async Task AddAsync_WithNullTemplate_ShouldReturnValidationError()
        {
            // Act
            var result = await _repository.AddAsync(null);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.Errors.Any(e => e.Code == "AbilityTemplate.Add.Failed"));
            Assert.Null(result.Data);
        }

        /// <summary>
        /// Tests conflict error when adding template with duplicate name
        /// </summary>
        [Fact]
        public async Task AddAsync_WithDuplicateName_ShouldReturnConflictError()
        {
            // Arrange
            var existingTemplate = CreateTestTemplate("Duplicate Ability", "Existing ability");
            await _context.AbilityTemplates.AddAsync(existingTemplate);
            await _context.SaveChangesAsync();

            var duplicateTemplate = CreateTestTemplate("Duplicate Ability", "New ability with same name");

            // Act
            var result = await _repository.AddAsync(duplicateTemplate);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.Errors.Any(e => e.Code == "Validation.Failed"));
            Assert.Contains("already exists", result.FirstErrorMessage);
            Assert.Null(result.Data);
        }

        /// <summary>
        /// Tests case insensitive duplicate name detection
        /// </summary>
        [Fact]
        public async Task AddAsync_WithDuplicateNameCaseInsensitive_ShouldReturnConflictError()
        {
            // Arrange
            var existingTemplate = CreateTestTemplate("Test Ability", "Existing ability");
            await _context.AbilityTemplates.AddAsync(existingTemplate);
            await _context.SaveChangesAsync();

            var duplicateTemplate = CreateTestTemplate("TEST ABILITY", "New ability with same name");

            // Act
            var result = await _repository.AddAsync(duplicateTemplate);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.Errors.Any(e => e.Code == "Validation.Failed"));
            Assert.Contains("already exists", result.FirstErrorMessage);
            Assert.Null(result.Data);
        }

        /// <summary>
        /// Tests successful addition of complex ability template with effects
        /// </summary>
        [Fact]
        public async Task AddAsync_WithComplexTemplate_ShouldPreserveAllProperties()
        {
            // Arrange
            var template = CreateTestTemplateWithEffects("Complex Ability", "Complex ability with effects");

            // Act
            var result = await _repository.AddAsync(template);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);

            var savedTemplate = await _context.AbilityTemplates.FindAsync(template.Id);
            Assert.NotNull(savedTemplate);
            Assert.Equal("Complex Ability", savedTemplate.Name);
            Assert.Equal(2, savedTemplate.Effects.Count);
            Assert.Equal("fire_explosion", savedTemplate.AnimationName);
            Assert.Equal("explosion_sound", savedTemplate.SoundEffect);
            Assert.Equal(2, savedTemplate.Requirements.Count);
        }

        #endregion

        #region UpdateAsync Tests

        /// <summary>
        /// Tests successful update of valid ability template
        /// </summary>
        [Fact]
        public async Task UpdateAsync_WithValidTemplate_ShouldUpdateSuccessfully()
        {
            // Arrange
            var template = CreateTestTemplate("Original Ability", "Original description");
            await _context.AbilityTemplates.AddAsync(template);
            await _context.SaveChangesAsync();

            // Modify the template
            var updatedTemplate = new AbilityTemplate("Updated Ability", "Updated description",
                AbilityType.Active, TargetType.SingleEnemy, 20, 3, 2);
            // Set the same ID to simulate update
            var idProperty = typeof(AbilityTemplate).BaseType.GetProperty("Id");
            idProperty.SetValue(updatedTemplate, template.Id);

            // Act
            var result = await _repository.UpdateAsync(updatedTemplate);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal("Updated Ability", result.Data.Name);

            // Verify database was updated
            var dbTemplate = await _context.AbilityTemplates.FindAsync(template.Id);
            Assert.NotNull(dbTemplate);
            Assert.Equal("Updated Ability", dbTemplate.Name);
            Assert.Equal("Updated description", dbTemplate.Description);
        }

        /// <summary>
        /// Tests validation error when updating with null template
        /// </summary>
        [Fact]
        public async Task UpdateAsync_WithNullTemplate_ShouldReturnValidationError()
        {
            // Act
            var result = await _repository.UpdateAsync(null);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.Errors.Any(e => e.Code == "AbilityTemplate.Update.Failed"));
            Assert.Null(result.Data);
        }

        /// <summary>
        /// Tests not found error when updating non-existent template
        /// </summary>
        [Fact]
        public async Task UpdateAsync_WithNonExistentTemplate_ShouldReturnNotFoundError()
        {
            // Arrange
            var template = CreateTestTemplate("Non Existent", "Non existent ability");

            // Act
            var result = await _repository.UpdateAsync(template);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.Errors.Any(e => e.Code == "NotFound"));
            Assert.Null(result.Data);
        }

        /// <summary>
        /// Tests conflict error when updating with duplicate name from other template
        /// </summary>
        [Fact]
        public async Task UpdateAsync_WithDuplicateNameFromOtherTemplate_ShouldReturnConflictError()
        {
            // Arrange
            var template1 = CreateTestTemplate("Ability 1", "First ability");
            var template2 = CreateTestTemplate("Ability 2", "Second ability");

            await _context.AbilityTemplates.AddRangeAsync(template1, template2);
            await _context.SaveChangesAsync();

            // Try to update template2 to have same name as template1
            var updatedTemplate = new AbilityTemplate("Ability 1", "Updated description",
                AbilityType.Active, TargetType.SingleEnemy);
            var idProperty = typeof(AbilityTemplate).BaseType.GetProperty("Id");
            idProperty.SetValue(updatedTemplate, template2.Id);

            // Act
            var result = await _repository.UpdateAsync(updatedTemplate);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.Errors.Any(e => e.Code == "Validation.Failed"));
            Assert.Contains("already exists", result.FirstErrorMessage);
            Assert.Null(result.Data);
        }

        /// <summary>
        /// Tests successful update with same name on same template
        /// </summary>
        [Fact]
        public async Task UpdateAsync_WithSameNameSameTemplate_ShouldUpdateSuccessfully()
        {
            // Arrange
            var template = CreateTestTemplate("Same Name Ability", "Original description");
            await _context.AbilityTemplates.AddAsync(template);
            await _context.SaveChangesAsync();

            // Update with same name but different description
            var updatedTemplate = new AbilityTemplate("Same Name Ability", "Updated description",
                AbilityType.Active, TargetType.SingleEnemy);
            var idProperty = typeof(AbilityTemplate).BaseType.GetProperty("Id");
            idProperty.SetValue(updatedTemplate, template.Id);

            // Act
            var result = await _repository.UpdateAsync(updatedTemplate);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal("Same Name Ability", result.Data.Name);
            Assert.Equal("Updated description", result.Data.Description);
        }

        #endregion

        #region DeleteAsync Entity Tests

        /// <summary>
        /// Tests successful deletion of valid entity
        /// </summary>
        [Fact]
        public async Task DeleteAsync_WithValidEntity_ShouldDeleteSuccessfully()
        {
            // Arrange
            var template = CreateTestTemplate("Delete Ability", "Ability to delete");
            await _context.AbilityTemplates.AddAsync(template);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteAsync(template);

            // Assert
            Assert.True(result.Succeeded);

            // Verify it was deleted from database
            var deletedTemplate = await _context.AbilityTemplates.FindAsync(template.Id);
            Assert.Null(deletedTemplate);
        }

        /// <summary>
        /// Tests validation error when deleting null entity
        /// </summary>
        [Fact]
        public async Task DeleteAsync_WithNullEntity_ShouldReturnValidationError()
        {
            // Act
            var result = await _repository.DeleteAsync((AbilityTemplate)null);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.Errors.Any(e => e.Code == "Validation.Failed"));
            Assert.Contains("Ability template cannot be null", result.FirstErrorMessage);
        }

        /// <summary>
        /// Tests not found error when deleting non-existent entity
        /// </summary>
        [Fact]
        public async Task DeleteAsync_WithNonExistentEntity_ShouldReturnNotFoundError()
        {
            // Arrange
            var template = CreateTestTemplate("Non Existent", "Non existent ability");

            // Act
            var result = await _repository.DeleteAsync(template);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.Errors.Any(e => e.Code == "AbilityTemplate.NotFound"));
        }

        #endregion

        #region DeleteAsync ID Tests

        /// <summary>
        /// Tests successful deletion by valid ID
        /// </summary>
        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldDeleteSuccessfully()
        {
            // Arrange
            var template = CreateTestTemplate("Delete By ID Ability", "Ability to delete by ID");
            await _context.AbilityTemplates.AddAsync(template);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteAsync(template.Id);

            // Assert
            Assert.True(result.Succeeded);

            // Verify it was deleted from database
            var deletedTemplate = await _context.AbilityTemplates.FindAsync(template.Id);
            Assert.Null(deletedTemplate);
        }

        /// <summary>
        /// Tests validation error when deleting with empty ID
        /// </summary>
        [Fact]
        public async Task DeleteAsync_WithEmptyId_ShouldReturnValidationError()
        {
            // Act
            var result = await _repository.DeleteAsync(Guid.Empty);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.Errors.Any(e => e.Code == "Validation.Failed"));
            Assert.Contains("ID cannot be empty", result.FirstErrorMessage);
        }

        /// <summary>
        /// Tests not found error when deleting with non-existent ID
        /// </summary>
        [Fact]
        public async Task DeleteAsync_WithNonExistentId_ShouldReturnNotFoundError()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.DeleteAsync(nonExistentId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.Errors.Any(e => e.Code == "AbilityTemplate.NotFound"));
        }

        #endregion

        #endregion

        #region Exception Handling Tests

        /// <summary>
        /// Tests exception handling in GetByAbilityTypeAsync with database error
        /// </summary>
        [Fact]
        public async Task GetByAbilityTypeAsync_WithDatabaseException_ShouldReturnFailureResult()
        {
            // Arrange
            _context.Dispose(); // Force database connection error

            // Act
            var result = await _repository.GetByAbilityTypeAsync(AbilityType.Active);

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotNull(result.Errors);
            Assert.Null(result.Data);
        }

        /// <summary>
        /// Tests exception handling in GetByTargetTypeAsync with database error
        /// </summary>
        [Fact]
        public async Task GetByTargetTypeAsync_WithDatabaseException_ShouldReturnFailureResult()
        {
            // Arrange
            _context.Dispose(); // Force database connection error

            // Act
            var result = await _repository.GetByTargetTypeAsync(TargetType.SingleEnemy);

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotNull(result.Errors);
            Assert.Null(result.Data);
        }

        /// <summary>
        /// Tests exception handling in GetByNameAsync with database error
        /// </summary>
        [Fact]
        public async Task GetByNameAsync_WithDatabaseException_ShouldReturnFailureResult()
        {
            // Arrange
            _context.Dispose(); // Force database connection error

            // Act
            var result = await _repository.GetByNameAsync("Test Ability");

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotNull(result.Errors);
            Assert.Null(result.Data);
        }

        /// <summary>
        /// Tests exception handling in AddAsync with database error
        /// </summary>
        [Fact]
        public async Task AddAsync_WithDatabaseException_ShouldReturnFailureResult()
        {
            // Arrange
            var template = CreateTestTemplate("Test Ability", "Test description");
            _context.Dispose(); // Force database connection error

            // Act
            var result = await _repository.AddAsync(template);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.Errors.Any(e => e.Code == "AbilityTemplate.Add.Failed"));
            Assert.Null(result.Data);
        }

        #endregion

        #region Edge Cases and Complex Scenarios

        /// <summary>
        /// Tests all AbilityType enum values for comprehensive coverage
        /// </summary>
        [Fact]
        public async Task GetByAbilityTypeAsync_WithAllEnumValues_ShouldHandleAllCases()
        {
            // Arrange
            var activeAbility = CreateTestTemplate("Active", "Active ability", AbilityType.Active);
            var passiveAbility = CreateTestTemplate("Passive", "Passive ability", AbilityType.Passive);
            var toggleAbility = CreateTestTemplate("Toggle", "Toggle ability", AbilityType.Toggle);
            var channeledAbility = CreateTestTemplate("Channeled", "Channeled ability", AbilityType.Channeled);

            await _context.AbilityTemplates.AddRangeAsync(activeAbility, passiveAbility, toggleAbility, channeledAbility);
            await _context.SaveChangesAsync();

            // Act & Assert for each enum value
            foreach (AbilityType abilityType in Enum.GetValues<AbilityType>())
            {
                var result = await _repository.GetByAbilityTypeAsync(abilityType);
                Assert.True(result.Succeeded);
                Assert.NotNull(result.Data);

                var abilities = result.Data.ToList();
                if (abilities.Any())
                {
                    Assert.All(abilities, a => Assert.Equal(abilityType, a.AbilityType));
                }
            }
        }

        /// <summary>
        /// Tests all TargetType enum values for comprehensive coverage
        /// </summary>
        [Fact]
        public async Task GetByTargetTypeAsync_WithAllEnumValues_ShouldHandleAllCases()
        {
            // Arrange
            var targets = Enum.GetValues<TargetType>().Take(5); // Take first 5 for testing
            var abilities = targets.Select((target, index) =>
                CreateTestTemplate($"Ability {index}", $"Ability with {target} target", AbilityType.Active, target))
                .ToList();

            await _context.AbilityTemplates.AddRangeAsync(abilities);
            await _context.SaveChangesAsync();

            // Act & Assert for each enum value
            foreach (TargetType targetType in Enum.GetValues<TargetType>())
            {
                var result = await _repository.GetByTargetTypeAsync(targetType);
                Assert.True(result.Succeeded);
                Assert.NotNull(result.Data);

                var matchingAbilities = result.Data.ToList();
                if (matchingAbilities.Any())
                {
                    Assert.All(matchingAbilities, a => Assert.Equal(targetType, a.TargetType));
                }
            }
        }

        /// <summary>
        /// Tests repository performance with large dataset
        /// </summary>
        [Fact]
        public async Task Repository_WithLargeDataset_ShouldPerformEfficiently()
        {
            // Arrange
            const int abilityCount = 100;
            var abilities = new List<AbilityTemplate>();

            for (int i = 0; i < abilityCount; i++)
            {
                var abilityType = (AbilityType)(i % 4); // Cycle through ability types
                var ability = CreateTestTemplate($"Ability {i:D3}", $"Description {i}", abilityType);
                abilities.Add(ability);
            }

            await _context.AbilityTemplates.AddRangeAsync(abilities);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByAbilityTypeAsync(AbilityType.Active);

            // Assert
            Assert.True(result.Succeeded);
            var activeAbilities = result.Data.ToList();
            Assert.Equal(25, activeAbilities.Count); // Should be 1/4 of total
            Assert.All(activeAbilities, a => Assert.Equal(AbilityType.Active, a.AbilityType));
        }

        /// <summary>
        /// Tests repository handling of special characters in names
        /// </summary>
        [Fact]
        public async Task Repository_WithSpecialCharactersInNames_ShouldHandleCorrectly()
        {
            // Arrange
            var specialNames = new[]
            {
                "Ability with @#$%",
                "Ability with 数字",
                "Ability with émojis 🎮",
                "Ability with \n newline",
                "Ability with \"quotes\"",
                "Ability with 'apostrophes'"
            };

            var abilities = specialNames.Select((name, index) =>
                CreateTestTemplate(name, $"Description {index}"))
                .ToList();

            await _context.AbilityTemplates.AddRangeAsync(abilities);
            await _context.SaveChangesAsync();

            // Act & Assert
            foreach (var name in specialNames)
            {
                var result = await _repository.GetByNameAsync(name);
                Assert.True(result.Succeeded, $"Failed to find ability with name: {name}");
                Assert.Equal(name, result.Data.Name);
            }
        }

        /// <summary>
        /// Tests complex ability template with all effect types
        /// </summary>
        [Fact]
        public async Task Repository_WithComplexAbilityEffects_ShouldPreserveAllData()
        {
            // Arrange
            var template = CreateTestTemplate("Ultimate Spell", "Complex spell with multiple effects");

            // Add various effect types
            var effects = new[]
            {
                new AbilityEffect(EffectType.PhysicalDamage, 50, 0),
                new AbilityEffect(EffectType.MagicalDamage, 30, 0),
                new AbilityEffect(EffectType.InstantHeal, 25, 0),
                new AbilityEffect(EffectType.Stun, 10, 3),
                new AbilityEffect(EffectType.StrengthBoost, 15, 10)
            };

            foreach (var effect in effects)
            {
                effect.SetParameter("multiplier", 1.5);
                effect.SetParameter("canCrit", true);
                template.AddEffect(effect);
            }

            template.SetVisuals("ultimate_animation", "ultimate_sound");
            template.AddRequirement("Level", 20);
            template.AddRequirement("Class", "Archmage");
            template.AddRequirement("Mana", 100);

            // Act
            var addResult = await _repository.AddAsync(template);
            Assert.True(addResult.Succeeded);

            var getResult = await _repository.GetByNameAsync("Ultimate Spell");

            // Assert
            Assert.True(getResult.Succeeded);
            var savedTemplate = getResult.Data;
            Assert.Equal(5, savedTemplate.Effects.Count);
            Assert.Equal("ultimate_animation", savedTemplate.AnimationName);
            Assert.Equal("ultimate_sound", savedTemplate.SoundEffect);
            Assert.Equal(3, savedTemplate.Requirements.Count);

            // Verify specific effect parameters
            var damageEffect = savedTemplate.Effects.First(e => e.EffectType == EffectType.PhysicalDamage);
            Assert.Equal(1.5, damageEffect.GetParameter<double>("multiplier"));
            Assert.True(damageEffect.GetParameter<bool>("canCrit"));
        }

        /// <summary>
        /// Tests GetAvailableForCharacterAsync with complex character requirements
        /// </summary>
        [Fact]
        public async Task GetAvailableForCharacterAsync_WithComplexRequirements_ShouldFilterCorrectly()
        {
            // Arrange
            var warriorAbility = CreateTestTemplate("Warrior Strike", "Warrior ability", AbilityType.Active, TargetType.SingleEnemy);
            warriorAbility.AddRequirement("Class", "Warrior");
            warriorAbility.AddRequirement("Level", 5);

            var mageAbility = CreateTestTemplate("Mage Spell", "Mage ability", AbilityType.Active, TargetType.Area);
            mageAbility.AddRequirement("Class", "Mage");
            mageAbility.AddRequirement("Level", 3);

            var anyClassAbility = CreateTestTemplate("Universal Skill", "Any class ability", AbilityType.Passive, TargetType.Self);

            await _context.AbilityTemplates.AddRangeAsync(warriorAbility, mageAbility, anyClassAbility);
            await _context.SaveChangesAsync();

            // Test character data for warrior
            var warriorData = new Dictionary<string, object>
            {
                { "Class", "Warrior" },
                { "Level", 5 }
            };

            // Act - Test abilities available to warrior
            var availableAbilities = new List<AbilityTemplate>();
            foreach (var template in await _context.AbilityTemplates.ToListAsync())
            {
                if (template.MeetsRequirements(warriorData))
                {
                    availableAbilities.Add(template);
                }
            }

            // Assert
            Assert.Single(availableAbilities);
            Assert.Equal("Warrior Strike", availableAbilities[0].Name);
        }

        #endregion
    }
}