using Moq;
using RpgGame.Application.Events;
using RpgGame.Application.Repositories;
using RpgGame.Application.Services;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Entities.Characters.Player;
using RpgGame.Domain.Entities.World;
using RpgGame.Domain.Events.Base;
using RpgGame.Domain.Interfaces.World;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RpgGame.UnitTests.Application.Services
{
    public class GameSaveServiceTests
    {
        private readonly Mock<IGameSaveRepository> _mockRepository;
        private readonly Mock<IEventStoreRepository> _mockEventStore;
        private readonly GameSaveService _gameSaveService;
        private readonly Character _testCharacter;
        private readonly ILocation _testLocation;
        private readonly GameWorld _gameWorld;

        public GameSaveServiceTests()
        {
            _mockRepository = new Mock<IGameSaveRepository>();
            _mockEventStore = new Mock<IEventStoreRepository>();
            _gameSaveService = new GameSaveService(_mockRepository.Object, _mockEventStore.Object);

            // Create test character
            _testCharacter = Warrior.Create("TestWarrior");

            // Create game world with test location
            _gameWorld = new GameWorld();
            _testLocation = _gameWorld.StartLocation;
        }

        [Fact]
        public async Task SaveGameAsync_ShouldSaveGameAndPublishEvent()
        {
            // Arrange
            string saveName = "TestSave";
            var domainEventMock = new Mock<IDomainEvent>(); // Create a mock for IDomainEvent
            _mockRepository.Setup(x => x.SaveGameAsync(
                saveName,
                _testCharacter,
                _testLocation.Name,
                It.IsAny<int>(),
                default))
                .ReturnsAsync(true);

            // Act
            bool result = await _gameSaveService.SaveGameAsync(saveName, _testCharacter, _testLocation);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(x => x.SaveGameAsync(
                saveName,
                _testCharacter,
                _testLocation.Name,
                It.IsAny<int>(),
                default),
                Times.Once);
            _mockEventStore.Verify(x => x.SaveEventAsync(domainEventMock.Object, null, default), Times.Once); // Use the mock object
        }

        [Fact]
        public async Task LoadGameAsync_ShouldReturnCharacterAndLocation()
        {
            // Arrange
            string saveName = "TestSave";
            int playTime = 300; // 5 minutes
            _mockRepository.Setup(x => x.LoadGameAsync(
                saveName,
                default))
                .ReturnsAsync((true, _testCharacter, _testLocation.Name, playTime));

            // Act
            var result = await _gameSaveService.LoadGameAsync(saveName, _gameWorld);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(_testCharacter, result.Player);
            Assert.Equal(_testLocation, result.CurrentLocation);
            _mockRepository.Verify(x => x.LoadGameAsync(saveName, default), Times.Once);
        }

        [Fact]
        public async Task LoadGameAsync_WithNonExistentLocation_ShouldUseStartLocation()
        {
            // Arrange
            string saveName = "TestSave";
            string nonExistentLocation = "NonExistentLocation";
            int playTime = 300;

            _mockRepository.Setup(x => x.LoadGameAsync(
                saveName,
                default))
                .ReturnsAsync((true, _testCharacter, nonExistentLocation, playTime));

            // Act
            var result = await _gameSaveService.LoadGameAsync(saveName, _gameWorld);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(_testCharacter, result.Player);
            Assert.Equal(_gameWorld.StartLocation, result.CurrentLocation);
            _mockRepository.Verify(x => x.LoadGameAsync(saveName, default), Times.Once);
        }

        [Fact]
        public void GetFormattedPlayTime_ShouldFormatTimeCorrectly()
        {
            // Arrange - Reset the session start time to ensure predictable results
            _gameSaveService.StartNewSession();

            // Set a known play time by using reflection to access private field
            var fieldInfo = typeof(GameSaveService).GetField("_currentPlayTime",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            fieldInfo?.SetValue(_gameSaveService, 3665); // 1h 1m 5s

            // Act
            string formattedTime = _gameSaveService.GetFormattedPlayTime();

            // Assert
            Assert.Equal("1h 1m 5s", formattedTime);
        }

        [Fact]
        public async Task GetAvailableSavesAsync_ShouldReturnSavesList()
        {
            // Arrange
            var expectedSaves = new List<(string SaveName, DateTime SaveDate)>
            {
                ("Save1", DateTime.Now.AddDays(-1)),
                ("Save2", DateTime.Now)
            };

            _mockRepository.Setup(x => x.GetAllSavesAsync(default))
                .ReturnsAsync(expectedSaves);

            // Act
            var saves = await _gameSaveService.GetAvailableSavesAsync();

            // Assert
            Assert.Equal(expectedSaves.Count, saves.Count);
            Assert.Equal(expectedSaves, saves);
            _mockRepository.Verify(x => x.GetAllSavesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteSaveAsync_ShouldDeleteSave()
        {
            // Arrange
            string saveName = "TestSave";
            _mockRepository.Setup(x => x.DeleteSaveAsync(saveName, default))
                .ReturnsAsync(true);

            // Act
            bool result = await _gameSaveService.DeleteSaveAsync(saveName);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(x => x.DeleteSaveAsync(saveName, default), Times.Once);
        }
    }
}