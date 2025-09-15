using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using RpgGame.Application.Commands.UserManagement;
using RpgGame.Application.Interfaces.Repositories;
using RpgGame.Domain.Common;
using RpgGame.Domain.Entities.Users;

namespace RpgGame.Application.Handlers.UserManagement
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, OperationResult>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CreateUserCommandHandler> _logger;

        public CreateUserCommandHandler(
            UserManager<IdentityUser> userManager,
            IUserRepository userRepository,
            ILogger<CreateUserCommandHandler> logger)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<OperationResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if username exists
                if (await _userRepository.UsernameExistsAsync(request.Username))
                {
                    return OperationResult.Failure("User.AlreadyExists", "Username already exists");
                }

                // Create Identity user
                var identityUser = new IdentityUser
                {
                    UserName = request.Username,
                    Email = request.Email,
                    EmailConfirmed = request.EmailConfirmed,
                    LockoutEnabled = request.LockoutEnabled
                };

                var result = await _userManager.CreateAsync(identityUser, request.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return OperationResult.Failure("User.CreateFailed", $"Failed to create user: {errors}");
                }

                // Create domain user
                var domainUser = User.Create(identityUser.Id, request.Username, request.Email);
                
                // Add roles
                foreach (var role in request.Roles)
                {
                    await _userManager.AddToRoleAsync(identityUser, role);
                    domainUser.AddRole(role);
                }

                await _userRepository.AddAsync(domainUser);

                _logger.LogInformation("User {Username} created successfully", request.Username);
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user {Username}", request.Username);
                return OperationResult.Failure("User.CreateError", $"Error creating user: {ex.Message}");
            }
        }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, OperationResult>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UpdateUserCommandHandler> _logger;

        public UpdateUserCommandHandler(
            UserManager<IdentityUser> userManager,
            IUserRepository userRepository,
            ILogger<UpdateUserCommandHandler> logger)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<OperationResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var identityUser = await _userManager.FindByIdAsync(request.Id);
                if (identityUser == null)
                {
                    return OperationResult.Failure("User.NotFound", "User not found");
                }

                var domainUser = await _userRepository.GetByAspNetUserIdAsync(request.Id);
                if (domainUser == null)
                {
                    return OperationResult.Failure("User.DomainNotFound", "Domain user not found");
                }

                // Update Identity user
                identityUser.UserName = request.Username;
                identityUser.Email = request.Email;
                identityUser.EmailConfirmed = request.EmailConfirmed;
                identityUser.LockoutEnabled = request.LockoutEnabled;
                identityUser.LockoutEnd = request.LockoutEnd;

                var result = await _userManager.UpdateAsync(identityUser);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return OperationResult.Failure("User.UpdateFailed", $"Failed to update user: {errors}");
                }

                // Update roles
                var currentRoles = await _userManager.GetRolesAsync(identityUser);
                var rolesToRemove = currentRoles.Except(request.Roles).ToList();
                var rolesToAdd = request.Roles.Except(currentRoles).ToList();

                if (rolesToRemove.Any())
                {
                    await _userManager.RemoveFromRolesAsync(identityUser, rolesToRemove);
                }

                if (rolesToAdd.Any())
                {
                    await _userManager.AddToRolesAsync(identityUser, rolesToAdd);
                }

                // Update domain user
                if (request.IsActive)
                {
                    domainUser.Reactivate("Admin", "User reactivated via admin panel");
                }
                else
                {
                    domainUser.Deactivate("Deactivated via admin panel", "Admin");
                }

                await _userRepository.UpdateAsync(domainUser);

                _logger.LogInformation("User {Username} updated successfully", request.Username);
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", request.Id);
                return OperationResult.Failure("User.UpdateError", $"Error updating user: {ex.Message}");
            }
        }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, OperationResult>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<DeleteUserCommandHandler> _logger;

        public DeleteUserCommandHandler(
            UserManager<IdentityUser> userManager,
            IUserRepository userRepository,
            ILogger<DeleteUserCommandHandler> logger)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<OperationResult> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var identityUser = await _userManager.FindByIdAsync(request.Id);
                if (identityUser == null)
                {
                    return OperationResult.Failure("User.NotFound", "User not found");
                }

                var domainUser = await _userRepository.GetByAspNetUserIdAsync(request.Id);
                if (domainUser != null)
                {
                    await _userRepository.DeleteAsync(domainUser);
                }

                var result = await _userManager.DeleteAsync(identityUser);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return OperationResult.Failure("User.DeleteFailed", $"Failed to delete user: {errors}");
                }

                _logger.LogInformation("User {Username} deleted successfully", identityUser.UserName);
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", request.Id);
                return OperationResult.Failure("User.DeleteError", $"Error deleting user: {ex.Message}");
            }
        }
    }

    public class ChangeUserPasswordCommandHandler : IRequestHandler<ChangeUserPasswordCommand, OperationResult>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ChangeUserPasswordCommandHandler> _logger;

        public ChangeUserPasswordCommandHandler(
            UserManager<IdentityUser> userManager,
            ILogger<ChangeUserPasswordCommandHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<OperationResult> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    return OperationResult.Failure("User.NotFound", "User not found");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return OperationResult.Failure("User.PasswordChangeFailed", $"Failed to change password: {errors}");
                }

                _logger.LogInformation("Password changed for user {Username}", user.UserName);
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {UserId}", request.UserId);
                return OperationResult.Failure("User.PasswordChangeError", $"Error changing password: {ex.Message}");
            }
        }
    }

    public class LockUserCommandHandler : IRequestHandler<LockUserCommand, OperationResult>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LockUserCommandHandler> _logger;

        public LockUserCommandHandler(
            UserManager<IdentityUser> userManager,
            ILogger<LockUserCommandHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<OperationResult> Handle(LockUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    return OperationResult.Failure("User.NotFound", "User not found");
                }

                var result = await _userManager.SetLockoutEndDateAsync(user, request.LockoutEnd);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return OperationResult.Failure("User.LockFailed", $"Failed to lock user: {errors}");
                }

                _logger.LogInformation("User {Username} locked until {LockoutEnd}. Reason: {Reason}", 
                    user.UserName, request.LockoutEnd, request.Reason);
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error locking user {UserId}", request.UserId);
                return OperationResult.Failure("User.LockError", $"Error locking user: {ex.Message}");
            }
        }
    }

    public class UnlockUserCommandHandler : IRequestHandler<UnlockUserCommand, OperationResult>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<UnlockUserCommandHandler> _logger;

        public UnlockUserCommandHandler(
            UserManager<IdentityUser> userManager,
            ILogger<UnlockUserCommandHandler> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<OperationResult> Handle(UnlockUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    return OperationResult.Failure("User.NotFound", "User not found");
                }

                var result = await _userManager.SetLockoutEndDateAsync(user, null);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return OperationResult.Failure("User.UnlockFailed", $"Failed to unlock user: {errors}");
                }

                _logger.LogInformation("User {Username} unlocked", user.UserName);
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unlocking user {UserId}", request.UserId);
                return OperationResult.Failure("User.UnlockError", $"Error unlocking user: {ex.Message}");
            }
        }
    }

    public class AssignRoleToUserCommandHandler : IRequestHandler<AssignRoleToUserCommand, OperationResult>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AssignRoleToUserCommandHandler> _logger;

        public AssignRoleToUserCommandHandler(
            UserManager<IdentityUser> userManager,
            IUserRepository userRepository,
            ILogger<AssignRoleToUserCommandHandler> logger)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<OperationResult> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    return OperationResult.Failure("User.NotFound", "User not found");
                }

                var result = await _userManager.AddToRoleAsync(user, request.RoleName);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return OperationResult.Failure("User.RoleAssignFailed", $"Failed to assign role: {errors}");
                }

                var domainUser = await _userRepository.GetByAspNetUserIdAsync(request.UserId);
                if (domainUser != null)
                {
                    domainUser.AddRole(request.RoleName);
                    await _userRepository.UpdateAsync(domainUser);
                }

                _logger.LogInformation("Role {RoleName} assigned to user {Username}", request.RoleName, user.UserName);
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role {RoleName} to user {UserId}", request.RoleName, request.UserId);
                return OperationResult.Failure("User.RoleAssignError", $"Error assigning role: {ex.Message}");
            }
        }
    }

    public class RemoveRoleFromUserCommandHandler : IRequestHandler<RemoveRoleFromUserCommand, OperationResult>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<RemoveRoleFromUserCommandHandler> _logger;

        public RemoveRoleFromUserCommandHandler(
            UserManager<IdentityUser> userManager,
            IUserRepository userRepository,
            ILogger<RemoveRoleFromUserCommandHandler> logger)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<OperationResult> Handle(RemoveRoleFromUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    return OperationResult.Failure("User.NotFound", "User not found");
                }

                var result = await _userManager.RemoveFromRoleAsync(user, request.RoleName);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return OperationResult.Failure("User.RoleRemoveFailed", $"Failed to remove role: {errors}");
                }

                var domainUser = await _userRepository.GetByAspNetUserIdAsync(request.UserId);
                if (domainUser != null)
                {
                    // Remove role from domain user (you might need to add this method)
                    await _userRepository.UpdateAsync(domainUser);
                }

                _logger.LogInformation("Role {RoleName} removed from user {Username}", request.RoleName, user.UserName);
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing role {RoleName} from user {UserId}", request.RoleName, request.UserId);
                return OperationResult.Failure("User.RoleRemoveError", $"Error removing role: {ex.Message}");
            }
        }
    }
}