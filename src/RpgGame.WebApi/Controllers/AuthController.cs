using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RpgGame.Application.Commands.Users;
using RpgGame.WebApi.DTOs.Auth;

namespace RpgGame.WebApi.Controllers
{
    /// <summary>
    /// Controller for managing authentication and user account operations
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [AllowAnonymous]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator instance used to handle application commands and queries. Cannot be <see langword="null"/>.</param>
        /// <param name="logger">The logger instance used for logging operations within the controller. Cannot be <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="mediator"/> or <paramref name="logger"/> is <see langword="null"/>.</exception>
        public AuthController(IMediator mediator, ILogger<AuthController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles user registration by processing the provided registration request.
        /// </summary>
        /// <remarks>This method validates the registration request, processes it using a mediator
        /// command,  and logs the outcome. If the registration fails, the response includes the specific  errors
        /// encountered.</remarks>
        /// <param name="request">The registration request containing the user's username and password.  This parameter cannot be <see
        /// langword="null"/>.</param>
        /// <returns>An <see cref="ActionResult{T}"/> containing an <see cref="AuthResponse"/> object.  If the registration is
        /// successful, the response indicates success with a message.  If the registration fails, the response contains
        /// error details.</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                _logger.LogInformation("Processing registration request for request: {RequestJson}", JsonConvert.SerializeObject(request, Formatting.Indented));

                // Check model state validation
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .SelectMany(x => x.Value!.Errors)
                        .Select(x => x.ErrorMessage)
                        .ToList();

                    _logger.LogWarning("Registration validation failed for email: {Email}. Errors: {Errors}", 
                        request.Email, string.Join(", ", errors));

                    return BadRequest(new AuthResponse
                    {
                        IsSuccess = false,
                        Errors = errors
                    });
                }

                var command = new RegisterUserCommand(request.Username, request.Email, request.Password);

                var result = await _mediator.Send(command);

                _logger.LogInformation("Registration result for email {Email}: {Result}", request.Email, JsonConvert.SerializeObject(result, Formatting.Indented));

                if (result.Succeeded)
                {
                    _logger.LogInformation("Registration successful for email: {Email}", request.Email);
                    
                    return Ok(new AuthResponse
                    {
                        IsSuccess = true,
                        AccessToken = result.AccessToken,
                        RefreshToken = result.RefreshToken,
                        ExpiresAt = result.ExpiresAt,
                        User = new UserResponse
                        {
                            Id = result.User.Id,
                            Email = result.User.Email,
                            Username = result.User.Username,
                            Roles = result.User.Roles
                        }
                    });
                }
                else
                {
                    _logger.LogWarning("Registration failed for email: {Email}. Errors: {Errors}", 
                        request.Email, string.Join(", ", result.Errors));
                    
                    return BadRequest(new AuthResponse
                    {
                        IsSuccess = false,
                        Errors = result.Errors
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing registration request for email: {Email}", request.Email);
                return StatusCode(500, new AuthResponse
                {
                    IsSuccess = false,
                    Errors = new List<string> { "An unexpected error occurred while processing your request." }
                });
            }
        }

        /// <summary>
        /// Handles user login by processing the provided login request.
        /// </summary>
        /// <remarks>This method validates the login request, processes it using a mediator
        /// command, and logs the outcome. If the login fails, the response includes the specific errors
        /// encountered.</remarks>
        /// <param name="request">The login request containing the user's email and password. This parameter cannot be <see
        /// langword="null"/>.</param>
        /// <returns>An <see cref="ActionResult{T}"/> containing an <see cref="AuthResponse"/> object. If the login is
        /// successful, the response includes the access token, refresh token, and user information. If the login fails, the response contains
        /// error details.</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                _logger.LogInformation("Processing login request for email: {Email}", request.Email);

                var command = new LoginCommand(request.Email, request.Password);

                var result = await _mediator.Send(command);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Login successful for email: {Email}", request.Email);
                    
                    return Ok(new AuthResponse
                    {
                        IsSuccess = true,
                        AccessToken = result.AccessToken,
                        RefreshToken = result.RefreshToken,
                        ExpiresAt = result.ExpiresAt,
                        User = new UserResponse
                        {
                            Id = result.User.Id,
                            Email = result.User.Email,
                            Username = result.User.Username,
                            Roles = result.User.Roles
                        }
                    });
                }
                else
                {
                    _logger.LogWarning("Login failed for email: {Email}. Errors: {Errors}", 
                        request.Email, string.Join(", ", result.Errors));
                    
                    return BadRequest(new AuthResponse
                    {
                        IsSuccess = false,
                        Errors = result.Errors
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing login request for email: {Email}", request.Email);
                return StatusCode(500, new AuthResponse
                {
                    IsSuccess = false,
                    Errors = new List<string> { "An unexpected error occurred while processing your request." }
                });
            }
        }

    }
}
