using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using RpgGame.WebApi.DTOs.UserManagement;
using RpgGame.Application.Commands.UserManagement;
using RpgGame.Application.Queries.UserManagement;

namespace RpgGame.WebApi.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(Policy = "Admin")]
    public class UserManagementController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(IMediator mediator, ILogger<UserManagementController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Test authentication and claims - returns current user info
        /// </summary>
        [HttpGet("test-auth")]
        public ActionResult TestAuth()
        {
            _logger.LogInformation("Test-auth endpoint called by user: {Username}, IsAuthenticated: {IsAuthenticated}", 
                User.Identity?.Name ?? "Anonymous", User.Identity?.IsAuthenticated ?? false);
                
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            var roles = User.Claims.Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                           .Select(c => c.Value).ToList();
                           
            _logger.LogDebug("User {Username} has {ClaimCount} claims and {RoleCount} roles", 
                User.Identity?.Name ?? "Anonymous", claims.Count, roles.Count);
            
            return Ok(new 
            {
                IsAuthenticated = User.Identity?.IsAuthenticated,
                Name = User.Identity?.Name,
                Claims = claims,
                Roles = roles,
                HasAdminRole = User.IsInRole("Admin"),
                AuthenticationType = User.Identity?.AuthenticationType
            });
        }

        /// <summary>
        /// Get all users with pagination and filtering
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<UserManagementDto>>> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? roleFilter = null)
        {
            try
            {
                var query = new GetAllUsersQuery(page, pageSize, searchTerm, roleFilter);
                var result = await _mediator.Send(query);

                var response = new
                {
                    Items = result.Items.Select(u => new UserManagementDto
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Email = u.Email,
                        EmailConfirmed = u.EmailConfirmed,
                        LockoutEnabled = u.LockoutEnabled,
                        LockoutEnd = u.LockoutEnd,
                        AccessFailedCount = u.AccessFailedCount,
                        Roles = u.Roles,
                        CreatedAt = u.CreatedAt,
                        LastLoginAt = u.LastLoginAt,
                        IsActive = u.IsActive
                    }).ToList(),
                    TotalCount = result.TotalCount,
                    Page = result.Page,
                    PageSize = result.PageSize,
                    TotalPages = result.TotalPages,
                    HasNextPage = result.HasNextPage,
                    HasPreviousPage = result.HasPreviousPage
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                return StatusCode(500, "Internal server error while retrieving users");
            }
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserManagementDto>> GetUser(string id)
        {
            try
            {
                var query = new GetUserByIdQuery(id);
                var result = await _mediator.Send(query);

                if (result == null)
                {
                    return NotFound($"User with ID {id} not found");
                }

                var response = new UserManagementDto
                {
                    Id = result.Id,
                    Username = result.Username,
                    Email = result.Email,
                    EmailConfirmed = result.EmailConfirmed,
                    LockoutEnabled = result.LockoutEnabled,
                    LockoutEnd = result.LockoutEnd,
                    AccessFailedCount = result.AccessFailedCount,
                    Roles = result.Roles,
                    CreatedAt = result.CreatedAt,
                    LastLoginAt = result.LastLoginAt,
                    IsActive = result.IsActive
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user {UserId}", id);
                return StatusCode(500, "Internal server error while retrieving user");
            }
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var command = new CreateUserCommand(
                    request.Username,
                    request.Email,
                    request.Password,
                    request.Roles,
                    request.EmailConfirmed,
                    request.LockoutEnabled
                );

                var result = await _mediator.Send(command);

                if (result.Succeeded)
                {
                    return Created($"/api/usermanagement/{request.Username}", new { message = "User created successfully" });
                }

                return BadRequest(new { message = result.FirstErrorMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user {Username}", request.Username);
                return StatusCode(500, "Internal server error while creating user");
            }
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(string id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != request.Id)
                {
                    return BadRequest("ID in URL does not match ID in request body");
                }

                var command = new UpdateUserCommand(
                    request.Id,
                    request.Username,
                    request.Email,
                    request.EmailConfirmed,
                    request.LockoutEnabled,
                    request.LockoutEnd,
                    request.Roles,
                    request.IsActive
                );

                var result = await _mediator.Send(command);

                if (result.Succeeded)
                {
                    return Ok(new { message = "User updated successfully" });
                }

                return BadRequest(new { message = result.FirstErrorMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", id);
                return StatusCode(500, "Internal server error while updating user");
            }
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            try
            {
                var command = new DeleteUserCommand(id);
                var result = await _mediator.Send(command);

                if (result.Succeeded)
                {
                    return Ok(new { message = "User deleted successfully" });
                }

                return BadRequest(new { message = result.FirstErrorMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", id);
                return StatusCode(500, "Internal server error while deleting user");
            }
        }

        /// <summary>
        /// Change user password
        /// </summary>
        [HttpPost("{id}/change-password")]
        public async Task<ActionResult> ChangePassword(string id, [FromBody] ChangePasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != request.UserId)
                {
                    return BadRequest("ID in URL does not match ID in request body");
                }

                var command = new ChangeUserPasswordCommand(request.UserId, request.NewPassword);
                var result = await _mediator.Send(command);

                if (result.Succeeded)
                {
                    return Ok(new { message = "Password changed successfully" });
                }

                return BadRequest(new { message = result.FirstErrorMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {UserId}", id);
                return StatusCode(500, "Internal server error while changing password");
            }
        }

        /// <summary>
        /// Lock a user account
        /// </summary>
        [HttpPost("{id}/lock")]
        public async Task<ActionResult> LockUser(string id, [FromBody] UserLockoutRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != request.UserId)
                {
                    return BadRequest("ID in URL does not match ID in request body");
                }

                var command = new LockUserCommand(request.UserId, request.LockoutEnd, request.Reason);
                var result = await _mediator.Send(command);

                if (result.Succeeded)
                {
                    return Ok(new { message = "User locked successfully" });
                }

                return BadRequest(new { message = result.FirstErrorMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error locking user {UserId}", id);
                return StatusCode(500, "Internal server error while locking user");
            }
        }

        /// <summary>
        /// Unlock a user account
        /// </summary>
        [HttpPost("{id}/unlock")]
        public async Task<ActionResult> UnlockUser(string id)
        {
            try
            {
                var command = new UnlockUserCommand(id);
                var result = await _mediator.Send(command);

                if (result.Succeeded)
                {
                    return Ok(new { message = "User unlocked successfully" });
                }

                return BadRequest(new { message = result.FirstErrorMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unlocking user {UserId}", id);
                return StatusCode(500, "Internal server error while unlocking user");
            }
        }

        /// <summary>
        /// Assign role to user
        /// </summary>
        [HttpPost("{id}/roles")]
        public async Task<ActionResult> AssignRole(string id, [FromBody] AssignRoleRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != request.UserId)
                {
                    return BadRequest("ID in URL does not match ID in request body");
                }

                var command = new AssignRoleToUserCommand(request.UserId, request.RoleName);
                var result = await _mediator.Send(command);

                if (result.Succeeded)
                {
                    return Ok(new { message = "Role assigned successfully" });
                }

                return BadRequest(new { message = result.FirstErrorMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role to user {UserId}", id);
                return StatusCode(500, "Internal server error while assigning role");
            }
        }

        /// <summary>
        /// Remove role from user
        /// </summary>
        [HttpDelete("{id}/roles")]
        public async Task<ActionResult> RemoveRole(string id, [FromBody] RemoveRoleRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != request.UserId)
                {
                    return BadRequest("ID in URL does not match ID in request body");
                }

                var command = new RemoveRoleFromUserCommand(request.UserId, request.RoleName);
                var result = await _mediator.Send(command);

                if (result.Succeeded)
                {
                    return Ok(new { message = "Role removed successfully" });
                }

                return BadRequest(new { message = result.FirstErrorMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing role from user {UserId}", id);
                return StatusCode(500, "Internal server error while removing role");
            }
        }
    }
}