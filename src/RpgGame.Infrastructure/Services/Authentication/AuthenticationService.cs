using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using RpgGame.Application.DTOs.Authentication;
using RpgGame.Application.Interfaces.Authentication;
using RpgGame.Application.Interfaces.Repositories;
using RpgGame.Domain.Common;

namespace RpgGame.Infrastructure.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IJwtTokenService jwtTokenService,
            IUserRepository userRepository,
            ILogger<AuthenticationService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<AuthenticationResult> RegisterAsync(string username, string email, string password)
        {
            try
            {
                ArgumentNullException.ThrowIfNullOrEmpty(username, nameof(username));
                ArgumentNullException.ThrowIfNullOrEmpty(email, nameof(email));
                ArgumentNullException.ThrowIfNullOrEmpty(password, nameof(password));

                _logger.LogInformation("Attempting to register user with username: {Username}, email: {Email}", username, email);

                var identityUser = new IdentityUser
                {
                    UserName = username,
                    Email = email
                };

                var result = await _userManager.CreateAsync(identityUser, password);

                if (!result.Succeeded)
                {
                    // Map IdentityError to OperationError
                    var operationErrors = result.Errors.Select(e => new OperationError(e.Code, e.Description));
                    _logger.LogWarning("User registration failed for {Username}. Errors: {Errors}",
                        username, string.Join(", ", operationErrors.Select(e => e.Description)));

                    return new AuthenticationResult(operationErrors);
                }

                // Add default role
                await _userManager.AddToRoleAsync(identityUser, "Player");

                return await GenerateAuthenticationResult(identityUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering the user {Username}", username);
                return new AuthenticationResult(new OperationError("RegistrationError", "An error occurred while registering the user."));
            }
        }

        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new AuthenticationResult(new OperationError("UserNotFound", "Invalid email or password"));
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return new AuthenticationResult(new OperationError("InvalidCredentials", "Invalid email or password"));
            }

            return await GenerateAuthenticationResult(user);
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string refreshToken)
        {
            var principal = _jwtTokenService.GetPrincipalFromExpiredToken(refreshToken);
            var userId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null || !await _jwtTokenService.ValidateRefreshTokenAsync(refreshToken, userId))
            {
                return new AuthenticationResult(new OperationError("InvalidRefreshToken", "Invalid refresh token"));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthenticationResult(new OperationError("UserNotFound", "User not found"));
            }

            // Revoke old refresh token and generate new tokens
            await _jwtTokenService.RevokeRefreshTokenAsync(refreshToken);
            return await GenerateAuthenticationResult(user);
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            return await _jwtTokenService.ValidateTokenAsync(token);
        }

        public async Task LogoutAsync(string userId)
        {
            // Revoke all refresh tokens for this user
            // Implementation depends on your refresh token storage strategyBeginnings are such delicate times
            await Task.CompletedTask;
        }

        private async Task<AuthenticationResult> GenerateAuthenticationResult(IdentityUser identityUser)
        {
            var roles = await _userManager.GetRolesAsync(identityUser);
            var userDto = new UserDto
            {
                Id = identityUser.Id,
                Username = identityUser.UserName!,
                Email = identityUser.Email!,
                Roles = roles.ToList()
            };

            var accessToken = _jwtTokenService.GenerateAccessToken(userDto);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();
            var expiresAt = DateTime.UtcNow.AddMinutes(60); // Match your JWT expiry

            await _jwtTokenService.StoreRefreshTokenAsync(refreshToken, identityUser.Id, expiresAt.AddDays(7));

            return new AuthenticationResult(true)
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                User = userDto
            };
        }
    }
}