using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RpgGame.Application.Commands.Users;
using RpgGame.Application.DTOs.Authentication;
using RpgGame.Application.Interfaces.Authentication;
using RpgGame.Application.Interfaces.Persistence;
using RpgGame.Application.Interfaces.Repositories;
using RpgGame.Domain.Common;
using RpgGame.Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RpgGame.Application.Handlers.Users
{
    /// <summary>
    /// Handler for the RegisterUserCommand.
    /// Orchestrates user registration through ASP.NET Core Identity and domain user creation.
    /// </summary>
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthenticationResult>
    {
        private readonly IAuthenticationService _authService;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RegisterUserCommandHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the RegisterUserCommandHandler.
        /// </summary>
        /// <param name="authService">The authentication service for handling ASP.NET Identity operations.</param>
        /// <param name="userRepository">The repository for domain user operations.</param>
        /// <param name="unitOfWork">The unit of work for transaction management.</param>
        /// <param name="logger">The logger for recording operations.</param>
        public RegisterUserCommandHandler(
            IAuthenticationService authService,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ILogger<RegisterUserCommandHandler> logger)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles the user registration command.
        /// </summary>
        /// <param name="request">The registration command containing user details.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The authentication result with tokens if successful.</returns>
        public async Task<AuthenticationResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing registration for username: {Username}, email: {Email}", request.Username, request.Email);
            try
            {
                // Step 1: Check if username already exists in domain
                if (await _userRepository.UsernameExistsAsync(request.Username))
                {
                    _logger.LogWarning("Username {Username} already exists.", request.Username);
                    return new AuthenticationResult(false, new List<OperationError> { new OperationError("Username already exists", "The username is already taken.") });
                }
                _logger.LogInformation("Username {Username} is available.", request.Username);

                // Step 2: Register user through ASP.NET Identity first
                _logger.LogInformation("Registering user with ASP.NET Identity: {Username}, email: {Email}", request.Username, request.Email);
                var identityResult = await _authService.RegisterAsync(request.Username, request.Email, request.Password);
                _logger.LogInformation("Identity Result Json: {Json}", JsonConvert.SerializeObject(identityResult, Formatting.Indented));
                if (!identityResult.Succeeded)
                {
                    _logger.LogWarning("ASP.NET Identity registration failed for {Username}. Errors: {Errors}",
                        request.Username, string.Join(", ", identityResult.Errors));
                    return identityResult;
                }

                // Step 3: Create user in the domain
                _logger.LogInformation("Creating domain user for ASP.NET Identity user ID: {UserId}", identityResult.User.Id);
                var user = User.Create(aspNetUserId: identityResult.User.Id, username: request.Username, email: request.Email);

                // Step 4: Save domain user to the repository
                _logger.LogInformation("Saving domain user: {Username}, email: {Email}", request.Username, request.Email);
                await _userRepository.AddAsync(user);

                // Step 5: Commit the transaction
                _logger.LogInformation("Committing transaction for user registration: {Username}, email: {Email}", request.Username, request.Email);
                var saveResult = await _unitOfWork.CommitAsync(cancellationToken);

                if (saveResult == false)
                {
                    _logger.LogError("Failed to save user {Username}, email: {Email}", request.Username, request.Email);
                    return new AuthenticationResult(false, new List<OperationError> { new OperationError("Registration failed", "Failed to complete user registration.") });
                }

                _logger.LogInformation("User registered successfully: {Username}, email: {Email}", request.Username, request.Email);

                // Domain events will be automatically dispatched by the UnitOfWork
                // This includes UserRegisteredEvent which triggers welcome emails, etc.

                return identityResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing registration for username: {Username}, email: {Email}", request.Username, request.Email);
                return new AuthenticationResult(false, new List<OperationError> { new OperationError("Registration failed", ex.Message) });
            }
        }
    }
}
