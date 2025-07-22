using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RpgGame.UnitTests.Helpers;
using RpgGame.WebApi.Controllers;
using RpgGame.WebApi.DTOs.Auth;
using Xunit.Abstractions;

namespace RpgGame.UnitTests.WebApi.Auth
{
    public class AuthControllerTests
    {
        private readonly ITestOutputHelper _output;
        public AuthControllerTests(ITestOutputHelper output) => _output = output;

        [Fact]
        public async Task Register_Returns200Ok_WhenRegistrationSucceeds()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            var logger = new XUnitLogger<AuthController>(_output);
            var controller = new AuthController(mediatorMock.Object, logger);

            var request = new RegisterRequest { Username = "dawidgrzejek", Email = "dawidgrzejek@gmail.com", Password = "Test123!", ConfirmPassword = "Test123!" };

            // Act
            var result = await controller.Register(request);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("test", okResult.Value);
        }
    }
}
