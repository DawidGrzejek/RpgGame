using Microsoft.EntityFrameworkCore;
using RpgGame.Application.Repositories;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Entities.World;
using RpgGame.Domain.ValueObjects;
using RpgGame.Infrastructure.Persistence.EFCore;
using RpgGame.Infrastructure.Persistence.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RpgGame.UnitTests.Infrastructure.Persistence
{
    public class GameSaveRepositoryTests
    {
        private readonly GameDbContext _context;
        private readonly GameSaveRepository _repository;
        private readonly string _testSaveName = "TestSave";
        private readonly Character _testCharacter;
        private readonly string _testLocationName = "TestLocation";
        private readonly int _testPlayTime = 600; // 10 minutes

        public GameSaveRepositoryTests()
        {
            // Create in-memory database for testing
            var options = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new GameDbContext(options);
            _repository = new GameSaveRepository(_context);

            // Create test character
            _testCharacter = Character.CreatePlayer("TestWarrior", RpgGame.Domain.Enums.PlayerClass.Warrior, 
                new CharacterStats(1, 100, 20, 10, 15, 5));
        }

        [Fact]
        public async Task SaveGameAsync_ShouldPersistNewGameSave()
        {
            // Act
            bool result = await _repository.SaveGameAsync(_testSaveName, _testCharacter, _testLocationName, _testPlayTime);

            // Assert
            Assert.True(result);
            var savedGame = await _context.GameSaves.FirstOrDefaultAsync(g => g.SaveName == _testSaveName);
            Assert.NotNull(savedGame);
            Assert.Equal(_testSaveName, savedGame.SaveName);
            Assert.Equal(_testLocationName, savedGame.CurrentLocationName);
            Assert.Equal(_testPlayTime, savedGame.PlayTime);
            // Can't easily compare characters directly since they're serialized
        }

        [Fact]
        public async Task SaveGameAsync_ShouldUpdateExistingSave()
        {
            // Arrange
            await _repository.SaveGameAsync(_testSaveName, _testCharacter, _testLocationName, _testPlayTime);

            string newLocationName = "NewLocation";
            int newPlayTime = 1200; // 20 minutes

            // Act
            bool result = await _repository.SaveGameAsync(_testSaveName, _testCharacter, newLocationName, newPlayTime);

            // Assert
            Assert.True(result);
            var savedGame = await _context.GameSaves.FirstOrDefaultAsync(g => g.SaveName == _testSaveName);
            Assert.NotNull(savedGame);
            Assert.Equal(_testSaveName, savedGame.SaveName);
            Assert.Equal(newLocationName, savedGame.CurrentLocationName);
            Assert.Equal(newPlayTime, savedGame.PlayTime);
        }

        [Fact]
        public async Task GetAllSavesAsync_ShouldReturnAllSaves()
        {
            // Arrange
            await _repository.SaveGameAsync("Save1", _testCharacter, _testLocationName, _testPlayTime);
            await _repository.SaveGameAsync("Save2", _testCharacter, _testLocationName, _testPlayTime);

            // Act
            var saves = await _repository.GetAllSavesAsync();

            // Assert
            Assert.Equal(2, saves.Count);
            Assert.Contains(saves, s => s.SaveName == "Save1");
            Assert.Contains(saves, s => s.SaveName == "Save2");
        }

        [Fact]
        public async Task DeleteSaveAsync_ShouldRemoveSave()
        {
            // Arrange
            await _repository.SaveGameAsync(_testSaveName, _testCharacter, _testLocationName, _testPlayTime);

            // Act
            bool result = await _repository.DeleteSaveAsync(_testSaveName);

            // Assert
            Assert.True(result);
            var savedGame = await _context.GameSaves.FirstOrDefaultAsync(g => g.SaveName == _testSaveName);
            Assert.Null(savedGame);
        }

        [Fact]
        public async Task DeleteSaveAsync_WithNonExistentSave_ShouldReturnFalse()
        {
            // Act
            bool result = await _repository.DeleteSaveAsync("NonExistentSave");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task LoadGameAsync_ShouldReturnGameState()
        {
            // Arrange
            await _repository.SaveGameAsync(_testSaveName, _testCharacter, _testLocationName, _testPlayTime);

            // Act
            var result = await _repository.LoadGameAsync(_testSaveName);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.PlayerCharacter);
            Assert.Equal(_testLocationName, result.CurrentLocationName);
            Assert.Equal(_testPlayTime, result.PlayTime);
        }

        [Fact]
        public async Task LoadGameAsync_WithNonExistentSave_ShouldReturnFalse()
        {
            // Act
            var result = await _repository.LoadGameAsync("NonExistentSave");

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.PlayerCharacter);
            Assert.Null(result.CurrentLocationName);
            Assert.Equal(0, result.PlayTime);
        }
    }
}