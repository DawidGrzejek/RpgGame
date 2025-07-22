using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using RpgGame.Application.Commands.Users;
using RpgGame.Application.Handlers.Users;
using RpgGame.Infrastructure.Persistence.EFCore;
using Xunit;

public class RegisterUserCommandHandlerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
  {
      private readonly WebApplicationFactory<Program> _factory;
      private readonly IServiceScope _scope;
      private readonly GameDbContext _dbContext;
      private readonly RegisterUserCommandHandler _handler;

      public RegisterUserCommandHandlerIntegrationTests(WebApplicationFactory<Program> factory)
      {
          _factory = factory;
          _scope = _factory.Services.CreateScope();
          _dbContext = _scope.ServiceProvider.GetRequiredService<GameDbContext>();
          _handler = _scope.ServiceProvider.GetRequiredService<RegisterUserCommandHandler>();
      }

      [Fact]
      public async Task Handle_CreatesUserInDatabase_WhenValidCommand()
      {
          // Arrange
          var command = new RegisterUserCommand(
              "integrationuser",
              "integration@example.com",
              "Test123!");

          // Act - This hits the REAL database and ASP.NET Identity
          var result = await _handler.Handle(command, CancellationToken.None);

          // Assert - Check both the result AND the database
          Assert.True(result.Succeeded);

          // Verify user was actually created in database
          var userInDb = await _dbContext.Users
              .FirstOrDefaultAsync(u => u.Username == "integrationuser");
          Assert.NotNull(userInDb);
          Assert.Equal("integration@example.com", userInDb.Email);
      }
  }