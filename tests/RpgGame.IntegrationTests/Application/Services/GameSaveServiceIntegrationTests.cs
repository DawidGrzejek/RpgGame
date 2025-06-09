using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RpgGame.Application.Events;
using RpgGame.Application.Interfaces.Services;
using RpgGame.Application.Repositories;
using RpgGame.Application.Services;
using RpgGame.Domain.Entities.Characters.Player;
using RpgGame.Domain.Entities.World;
using RpgGame.Domain.Interfaces.World;
using RpgGame.Infrastructure.Persistence.EFCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace RpgGame.IntegrationTests.Application.Services
{
    public class GameSaveServiceIntegrationTests : IDisposable
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly GameDbContext _dbContext;
        private readonly IGameSaveService _gameSaveService;
        private readonly IGameWorld _gameWorld;
        private readonly Warrior _testCharacter;
        private readonly string _testSaveName = "IntegrationTestSave";

        public GameSaveServiceIntegrationTests()
        {
            // Setup service collection
            var services = new ServiceCollection();

            // Add configuration
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();

            // Add DbContext using in-memory database
            services.AddDbContext<GameDbContext>(options =>
                options.UseInMemoryDatabase("GameSaveIntegrationTests_" + Guid.NewGuid().ToString()));

            // Register domain services
            services.AddSingleton<GameWorld>();
            services.AddSingleton<IGameWorld>(provider => provider.GetRequiredService<GameWorld>());

            // Register application services
            RpgGame.Application.DependencyInjection.AddGameSaveServices(services);

            // Register event store (simplified for tests)
            services.AddScoped<IEventStoreRepository, TestEventStoreRepository>();

            // Build service provider
            _serviceProvider = services.BuildServiceProvider();

            // Get required services
            _dbContext = _serviceProvider.GetRequiredService<GameDbContext>();
            _gameSaveService = _serviceProvider.GetRequiredService<IGameSaveService>();
            _gameWorld = _serviceProvider.GetRequiredService<IGameWorld>();

            // Create test character
            _testCharacter = Warrior.Create("IntegrationTestWarrior");
        }

        [Fact]
        public async Task SaveAndLoadGame_ShouldWorkEndToEnd()
        {
            // Arrange
            ILocation startLocation = _gameWorld.StartLocation;

            // Act - Save
            bool saveResult = await _gameSaveService.SaveGameAsync(_testSaveName, _testCharacter, startLocation);

            // Assert - Save
            Assert.True(saveResult);

            // Act - Load
            var loadResult = await _gameSaveService.LoadGameAsync(_testSaveName, _gameWorld);

            // Assert - Load
            Assert.True(loadResult.Success);
            Assert.NotNull(loadResult.Player);
            Assert.Equal(_testCharacter.Name, loadResult.Player.Name);
            Assert.Equal(startLocation.Name, loadResult.CurrentLocation.Name);
        }

        [Fact]
        public async Task SaveAndDeleteGame_ShouldWorkEndToEnd()
        {
            // Arrange
            await _gameSaveService.SaveGameAsync(_testSaveName, _testCharacter, _gameWorld.StartLocation);

            // Act - Delete
            bool deleteResult = await _gameSaveService.DeleteSaveAsync(_testSaveName);

            // Assert - Delete
            Assert.True(deleteResult);

            // Act - Try to load deleted save
            var loadResult = await _gameSaveService.LoadGameAsync(_testSaveName, _gameWorld);

            // Assert - Verify it's not found
            Assert.False(loadResult.Success);
            Assert.Null(loadResult.Player);
            Assert.Null(loadResult.CurrentLocation);
        }

        [Fact]
        public async Task GetAvailableSaves_ShouldReturnSaves()
        {
            // Arrange
            await _gameSaveService.SaveGameAsync("Save1", _testCharacter, _gameWorld.StartLocation);
            await _gameSaveService.SaveGameAsync("Save2", _testCharacter, _gameWorld.StartLocation);

            // Act
            var saves = await _gameSaveService.GetAvailableSavesAsync();

            // Assert
            Assert.Equal(2, saves.Count);
            Assert.Contains(saves, s => s.SaveName == "Save1");
            Assert.Contains(saves, s => s.SaveName == "Save2");
        }

        public void Dispose()
        {
            // Clean up
            _dbContext.Database.EnsureDeleted();
            _serviceProvider.Dispose();
        }
    }
}