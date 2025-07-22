using Microsoft.Extensions.DependencyInjection;
using RpgGame.Application.Interfaces.Services;
using RpgGame.Application.Services;
using RpgGame.IntegrationTests.Infrastructure;
using Xunit;

namespace RpgGame.IntegrationTests.Application.Services
{
    public class ServiceResolutionTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly TestWebApplicationFactory _factory;

        public ServiceResolutionTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public void ICharacterService_ShouldBeResolvable()
        {
            // Arrange & Act
            using var scope = _factory.Services.CreateScope();
            var characterService = scope.ServiceProvider.GetService<ICharacterService>();

            // Assert
            Assert.NotNull(characterService);
            Assert.IsType<CharacterService>(characterService);
        }

        [Fact]
        public void ICharacterService_ShouldBeRegisteredAsScoped()
        {
            // Arrange & Act
            using var scope1 = _factory.Services.CreateScope();
            using var scope2 = _factory.Services.CreateScope();
            
            var characterService1a = scope1.ServiceProvider.GetService<ICharacterService>();
            var characterService1b = scope1.ServiceProvider.GetService<ICharacterService>();
            var characterService2 = scope2.ServiceProvider.GetService<ICharacterService>();

            // Assert
            Assert.NotNull(characterService1a);
            Assert.NotNull(characterService1b);
            Assert.NotNull(characterService2);
            
            // Same instance within same scope
            Assert.Same(characterService1a, characterService1b);
            
            // Different instances across different scopes
            Assert.NotSame(characterService1a, characterService2);
        }

        [Fact]
        public void IGameSaveService_ShouldBeResolvable()
        {
            // Arrange & Act
            using var scope = _factory.Services.CreateScope();
            var gameSaveService = scope.ServiceProvider.GetService<IGameSaveService>();

            // Assert
            Assert.NotNull(gameSaveService);
            Assert.IsType<GameSaveService>(gameSaveService);
        }

        [Fact]
        public void IQuestService_ShouldBeResolvable()
        {
            // Arrange & Act
            using var scope = _factory.Services.CreateScope();
            var questService = scope.ServiceProvider.GetService<IQuestService>();

            // Assert
            Assert.NotNull(questService);
            Assert.IsType<QuestService>(questService);
        }

        [Fact]
        public void AllRequiredRepositories_ShouldBeResolvable()
        {
            // Arrange & Act
            using var scope = _factory.Services.CreateScope();
            
            var characterRepo = scope.ServiceProvider.GetService<RpgGame.Application.Interfaces.Repositories.ICharacterRepository>();
            var gameSaveRepo = scope.ServiceProvider.GetService<RpgGame.Application.Interfaces.Repositories.IGameSaveRepository>();
            var itemRepo = scope.ServiceProvider.GetService<RpgGame.Application.Interfaces.Repositories.IItemRepository>();

            // Assert
            Assert.NotNull(characterRepo);
            Assert.NotNull(gameSaveRepo);
            Assert.NotNull(itemRepo);
        }

        [Fact]
        public void CharactersController_Dependencies_ShouldAllBeResolvable()
        {
            // Arrange & Act
            using var scope = _factory.Services.CreateScope();
            
            // This tests that all dependencies needed by CharactersController can be resolved
            var characterService = scope.ServiceProvider.GetService<ICharacterService>();
            var mediator = scope.ServiceProvider.GetService<MediatR.IMediator>();

            // Assert
            Assert.NotNull(characterService);
            Assert.NotNull(mediator);
        }

        [Fact]
        public void CharactersController_ShouldBeCreatableViaHttpClient()
        {
            // Arrange & Act - This will fail if DI is not properly configured
            var client = _factory.CreateClient();
            
            // Making a request to the controller will test that it can be constructed
            // If ICharacterService couldn't be resolved, this would fail during controller construction
            var response = client.GetAsync("/api/v1/characters").Result;

            // Assert - We don't care about the response content, just that the controller could be created
            // The response might be 401 (Unauthorized) or 200 (OK), but not 500 (Internal Server Error due to DI issues)
            Assert.True(response.StatusCode != System.Net.HttpStatusCode.InternalServerError, 
                $"Controller construction failed. Status: {response.StatusCode}");
        }
    }
}