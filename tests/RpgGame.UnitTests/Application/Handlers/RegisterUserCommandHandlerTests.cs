using Microsoft.Extensions.Logging;
using Moq;
using RpgGame.Application.Commands.Users;
using RpgGame.Application.DTOs.Authentication;
using RpgGame.Application.Handlers.Users;
using RpgGame.Application.Interfaces.Authentication;
using RpgGame.Application.Interfaces.Persistence;
using RpgGame.Application.Interfaces.Repositories;
using RpgGame.Domain.Common;
using RpgGame.Domain.Entities.Users;
using RpgGame.UnitTests.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace RpgGame.UnitTests.Application.Handlers
{
    /// <summary>
    /// Tests for RegisterUserCommandHandler demonstrating TDD with mocking
    /// 
    /// UNDERSTANDING MOCKING IN TDD:
    /// 
    /// 1. WHAT IS MOCKING?
    /// - Mocking creates fake implementations of dependencies
    /// - We control what these fake objects return
    /// - We can verify how they were called
    /// 
    /// 2. WHY USE MOCKS?
    /// - Test in isolation (no database, no network calls)
    /// - Control dependencies' behavior
    /// - Verify interactions between objects
    /// - Fast execution
    /// 
    /// 3. KEY CONCEPTS:
    /// - Setup: Define what a mock should return
    /// - Verify: Check if a mock was called correctly
    /// - Behavior: Strict vs Loose mocking
    /// </summary>
    public class RegisterUserCommandHandlerTests
    {
        private readonly ITestOutputHelper _output;
        private readonly Mock<IAuthenticationService> _authServiceMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly XUnitLogger<RegisterUserCommandHandler> _logger;
        private readonly RegisterUserCommandHandler _handler;

        public RegisterUserCommandHandlerTests(ITestOutputHelper output)
        {
            _output = output;
            
            // Step 1: Create mocks for all dependencies
            // Mock<T> creates a fake implementation of interface T
            _authServiceMock = new Mock<IAuthenticationService>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _logger = new XUnitLogger<RegisterUserCommandHandler>(_output);
            
            // Step 2: Create the handler with mocked dependencies
            // The handler doesn't know these are fake - it just uses the interfaces
            _handler = new RegisterUserCommandHandler(
                _authServiceMock.Object,  // .Object gives us the fake implementation
                _userRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _logger);
        }

        [Fact]
        public async Task Handle_ReturnsSuccess_WhenUserRegistrationSucceeds()
        {
            // ARRANGE: Set up the test scenario
            var command = new RegisterUserCommand("testuser", "test@example.com", "Test123!");
            var aspNetUserId = "asp-net-user-id-123";
            var successResult = new AuthenticationResult(true)
            {
                User = new UserDto { Id = aspNetUserId }
            };

            // MOCK SETUP: Tell our mocks what to return when called
            // This is the power of mocking - we control the behavior!
            
            // When UsernameExistsAsync is called with "testuser", return false
            _userRepositoryMock
                .Setup(x => x.UsernameExistsAsync(command.Username))
                .ReturnsAsync(false);

            // When RegisterAsync is called with our parameters, return success
            _authServiceMock
                .Setup(x => x.RegisterAsync(command.Username, command.Email, command.Password))
                .ReturnsAsync(successResult);

            // When CommitAsync is called, return true (success)
            _unitOfWorkMock
                .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // ACT: Execute the method we're testing
            var result = await _handler.Handle(command, CancellationToken.None);

            // ASSERT: Verify the results
            Assert.True(result.Succeeded);
            Assert.Equal(aspNetUserId, result.User.Id);
            
            // VERIFY: Check that our mocks were called correctly
            // This ensures the handler follows the expected flow
            _userRepositoryMock.Verify(x => x.UsernameExistsAsync(command.Username), Times.Once);
            _authServiceMock.Verify(x => x.RegisterAsync(command.Username, command.Email, command.Password), Times.Once);
            _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsFailure_WhenUsernameAlreadyExists()
        {
            // ARRANGE
            var command = new RegisterUserCommand("existinguser", "test@example.com", "Test123!");

            // MOCK SETUP: Username already exists
            _userRepositoryMock
                .Setup(x => x.UsernameExistsAsync(command.Username))
                .ReturnsAsync(true);  // This will cause the handler to return early

            // ACT
            var result = await _handler.Handle(command, CancellationToken.None);

            // ASSERT
            Assert.False(result.Succeeded);
            
            // VERIFY: Check that the handler didn't continue after finding existing username
            _userRepositoryMock.Verify(x => x.UsernameExistsAsync(command.Username), Times.Once);
            _authServiceMock.Verify(x => x.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ReturnsFailure_WhenAuthServiceFails()
        {
            // ARRANGE
            var command = new RegisterUserCommand("testuser", "test@example.com", "Test123!");
            var failureResult = new AuthenticationResult(false);

            // MOCK SETUP: Auth service fails
            _userRepositoryMock
                .Setup(x => x.UsernameExistsAsync(command.Username))
                .ReturnsAsync(false);

            _authServiceMock
                .Setup(x => x.RegisterAsync(command.Username, command.Email, command.Password))
                .ReturnsAsync(failureResult);  // Return failure

            // ACT
            var result = await _handler.Handle(command, CancellationToken.None);

            // ASSERT
            Assert.False(result.Succeeded);
            
            // VERIFY: Domain user should not be created when auth fails
            _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ReturnsFailure_WhenCommitFails()
        {
            // ARRANGE
            var command = new RegisterUserCommand("testuser", "test@example.com", "Test123!");
            var successResult = new AuthenticationResult(true)
            {
                User = new UserDto { Id = "asp-net-user-id-123" }
            };

            // MOCK SETUP: Everything succeeds except the commit
            _userRepositoryMock
                .Setup(x => x.UsernameExistsAsync(command.Username))
                .ReturnsAsync(false);

            _authServiceMock
                .Setup(x => x.RegisterAsync(command.Username, command.Email, command.Password))
                .ReturnsAsync(successResult);

            _unitOfWorkMock
                .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);  // Commit fails

            // ACT
            var result = await _handler.Handle(command, CancellationToken.None);

            // ASSERT
            Assert.False(result.Succeeded);
            
            // VERIFY: All steps should be attempted
            _userRepositoryMock.Verify(x => x.UsernameExistsAsync(command.Username), Times.Once);
            _authServiceMock.Verify(x => x.RegisterAsync(command.Username, command.Email, command.Password), Times.Once);
            _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsFailure_WhenExceptionOccurs()
        {
            // ARRANGE
            var command = new RegisterUserCommand("testuser", "test@example.com", "Test123!");
            
            // MOCK SETUP: Make the repository throw an exception
            _userRepositoryMock
                .Setup(x => x.UsernameExistsAsync(command.Username))
                .ThrowsAsync(new InvalidOperationException("Database connection failed"));

            // ACT
            var result = await _handler.Handle(command, CancellationToken.None);

            // ASSERT
            Assert.False(result.Succeeded);
            
            // Exception handling should result in failure response
            // The handler catches exceptions and returns an error result
        }

        /// <summary>
        /// This test demonstrates different mock setups and verification patterns
        /// </summary>
        [Fact]
        public async Task MockingPatterns_Demonstration()
        {
            // DIFFERENT SETUP PATTERNS:
            
            // 1. Setup with specific parameters
            _userRepositoryMock
                .Setup(x => x.UsernameExistsAsync("specific-user"))
                .ReturnsAsync(true);

            // 2. Setup with any parameter using It.IsAny<T>()
            _userRepositoryMock
                .Setup(x => x.UsernameExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            // 3. Setup with conditional logic
            _userRepositoryMock
                .Setup(x => x.UsernameExistsAsync(It.Is<string>(s => s.StartsWith("admin"))))
                .ReturnsAsync(true);

            // 4. Setup to throw exception
            _userRepositoryMock
                .Setup(x => x.UsernameExistsAsync("error-user"))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // DIFFERENT VERIFICATION PATTERNS:
            
            // After calling the handler, you can verify:
            
            // 1. Method was called exactly once
            //_userRepositoryMock.Verify(x => x.UsernameExistsAsync("test"), Times.Once);
            
            // 2. Method was never called
            //_userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
            
            // 3. Method was called at least once
            //_userRepositoryMock.Verify(x => x.UsernameExistsAsync(It.IsAny<string>()), Times.AtLeastOnce);
            
            // 4. Method was called with specific parameters
            //_userRepositoryMock.Verify(x => x.UsernameExistsAsync("exact-match"), Times.Once);

            // This test doesn't actually test anything - it's just for demonstration
            Assert.True(true);
        }
    }
}