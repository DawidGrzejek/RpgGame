using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using RpgGame.Application.Commands.UserManagement;
using RpgGame.Domain.Common;

namespace RpgGame.Application.Handlers.UserManagement
{
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, OperationResult>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<CreateRoleCommandHandler> _logger;

        public CreateRoleCommandHandler(
            RoleManager<IdentityRole> roleManager,
            ILogger<CreateRoleCommandHandler> logger)
        {
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<OperationResult> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (await _roleManager.RoleExistsAsync(request.Name))
                {
                    return OperationResult.Failure("Role.AlreadyExists", "Role already exists");
                }

                var role = new IdentityRole(request.Name);
                var result = await _roleManager.CreateAsync(role);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return OperationResult.Failure("Role.CreateFailed", $"Failed to create role: {errors}");
                }

                _logger.LogInformation("Role {RoleName} created successfully", request.Name);
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role {RoleName}", request.Name);
                return OperationResult.Failure("Role.CreateError", $"Error creating role: {ex.Message}");
            }
        }
    }

    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, OperationResult>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UpdateRoleCommandHandler> _logger;

        public UpdateRoleCommandHandler(
            RoleManager<IdentityRole> roleManager,
            ILogger<UpdateRoleCommandHandler> logger)
        {
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<OperationResult> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(request.Id);
                if (role == null)
                {
                    return OperationResult.Failure("Role.NotFound", "Role not found");
                }

                var oldName = role.Name;
                role.Name = request.Name;
                role.NormalizedName = request.Name.ToUpper();

                var result = await _roleManager.UpdateAsync(role);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return OperationResult.Failure("Role.UpdateFailed", $"Failed to update role: {errors}");
                }

                _logger.LogInformation("Role {OldName} updated to {NewName}", oldName, request.Name);
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role {RoleId}", request.Id);
                return OperationResult.Failure("Role.UpdateError", $"Error updating role: {ex.Message}");
            }
        }
    }

    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, OperationResult>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<DeleteRoleCommandHandler> _logger;

        public DeleteRoleCommandHandler(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            ILogger<DeleteRoleCommandHandler> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<OperationResult> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(request.Id);
                if (role == null)
                {
                    return OperationResult.Failure("Role.NotFound", "Role not found");
                }

                // Check if any users have this role
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
                if (usersInRole.Any())
                {
                    return OperationResult.Failure("Role.InUse", $"Cannot delete role. {usersInRole.Count} users are assigned to this role.");
                }

                var result = await _roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return OperationResult.Failure("Role.DeleteFailed", $"Failed to delete role: {errors}");
                }

                _logger.LogInformation("Role {RoleName} deleted successfully", role.Name);
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role {RoleId}", request.Id);
                return OperationResult.Failure("Role.DeleteError", $"Error deleting role: {ex.Message}");
            }
        }
    }
}