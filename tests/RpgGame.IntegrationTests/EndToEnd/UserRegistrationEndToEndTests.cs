using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RpgGame.Application.Commands.Users;
using RpgGame.Application.Handlers.Users;
using RpgGame.Infrastructure.Persistence.EFCore;
using RpgGame.IntegrationTests.Infrastructure;
using RpgGame.WebApi.DTOs.Auth;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace RpgGame.IntegrationTests.EndToEnd
{
    /// <summary>
    /// End-to-end tests for user registration that test the complete flow:
    /// HTTP Request → Controller → Handler → Database
    /// </summary>
    public class UserRegistrationEndToEndTests : IClassFixture<TestWebApplicationFactory>, IAsyncLifetime
    {
        private readonly TestWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public UserRegistrationEndToEndTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        public async Task InitializeAsync()
        {
            // Reset database before each test
            await _factory.ResetDatabaseAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task RegisterUser_EndToEnd_Success()
        {
            // ARRANGE: Create registration request
            var uniqueId = Guid.NewGuid().ToString()[..8];
            var registerRequest = new RegisterRequest
            {
                Username = $"e2euser_{uniqueId}",
                Email = $"e2e_{uniqueId}@example.com",
                Password = "Test123!",
                ConfirmPassword = "Test123!"
            };

            // ACT: Send HTTP POST request to the registration endpoint
            var response = await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

            // ASSERT: Check HTTP response
            var responseContent = await response.Content.ReadAsStringAsync();
            
            // Debug: Show what we actually got
            Assert.True(false, $"Status: {response.StatusCode}, Content: {responseContent}");

            // VERIFY: Check that user was actually created in database
            using var dbContext = _factory.GetDbContext();
            var userInDb = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == registerRequest.Username);

            Assert.NotNull(userInDb);
            Assert.Equal(registerRequest.Email, userInDb.Email);
            Assert.True(userInDb.IsActive);
        }

        [Fact]
        public async Task RegisterUser_EndToEnd_FailsWhenUsernameExists()
        {
            // ARRANGE: Create a user first
            var username = $"duplicate_{Guid.NewGuid().ToString()[..8]}";
            var email1 = $"first_{Guid.NewGuid().ToString()[..8]}@example.com";
            var email2 = $"second_{Guid.NewGuid().ToString()[..8]}@example.com";

            var firstRequest = new RegisterRequest
            {
                Username = username,
                Email = email1,
                Password = "Test123!",
                ConfirmPassword = "Test123!"
            };

            // Create first user
            var firstResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", firstRequest);
            firstResponse.EnsureSuccessStatusCode();

            // ACT: Try to create second user with same username
            var duplicateRequest = new RegisterRequest
            {
                Username = username, // Same username
                Email = email2,      // Different email
                Password = "Test123!",
                ConfirmPassword = "Test123!"
            };

            var duplicateResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", duplicateRequest);

            // ASSERT: Should fail with conflict
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, duplicateResponse.StatusCode);

            // VERIFY: Only one user should exist in database
            using var dbContext = _factory.GetDbContext();
            var usersWithSameName = await dbContext.Users
                .Where(u => u.Username == username)
                .ToListAsync();

            Assert.Single(usersWithSameName);
            Assert.Equal(email1, usersWithSameName.First().Email); // Should be the first email
        }

        [Fact]
        public async Task RegisterUser_EndToEnd_FailsWithInvalidData()
        {
            // ARRANGE: Create invalid registration request
            var invalidRequest = new RegisterRequest
            {
                Username = "", // Empty username
                Email = "invalid-email", // Invalid email format
                Password = "123", // Too short password
                ConfirmPassword = "different" // Passwords don't match
            };

            // ACT: Send HTTP POST request
            var response = await _client.PostAsJsonAsync("/api/v1/auth/register", invalidRequest);

            // ASSERT: Should fail with validation error
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

            // VERIFY: No user should be created in database
            using var dbContext = _factory.GetDbContext();
            var userCount = await dbContext.Users.CountAsync();
            Assert.Equal(0, userCount);
        }

        [Fact]
        public async Task RegisterUser_ViaHandler_DirectDatabaseTest()
        {
            // This test bypasses HTTP and tests the handler directly
            // but still uses the real database and DI container

            // ARRANGE: Get services from DI container
            using var scope = _factory.Services.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<RegisterUserCommandHandler>();
            var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();

            var uniqueId = Guid.NewGuid().ToString()[..8];
            var command = new RegisterUserCommand(
                $"handleruser_{uniqueId}",
                $"handler_{uniqueId}@example.com",
                "Test123!");

            // ACT: Execute handler directly
            var result = await handler.Handle(command, CancellationToken.None);

            // ASSERT: Check result
            Assert.True(result.Succeeded);
            Assert.NotNull(result.User);

            // VERIFY: Check database
            var userInDb = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == $"handleruser_{uniqueId}");

            Assert.NotNull(userInDb);
            Assert.Equal($"handler_{uniqueId}@example.com", userInDb.Email);
        }
    }
}