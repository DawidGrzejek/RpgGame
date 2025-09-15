using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using RpgGame.WebApi.DTOs.UserManagement;
using RpgGame.Application.Commands.UserManagement;
using RpgGame.Application.Queries.UserManagement;
using ApplicationRoleDto = RpgGame.Application.Queries.UserManagement.RoleManagementDto;

namespace RpgGame.WebApi.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(Policy = "Admin")]
    public class RoleManagementController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RoleManagementController> _logger;

        public RoleManagementController(IMediator mediator, ILogger<RoleManagementController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get all roles
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<ApplicationRoleDto>>> GetRoles()
        {
            _logger.LogInformation("GetRoles called by user: {Username}, IsAuthenticated: {IsAuthenticated}, Roles: {Roles}",
                User.Identity?.Name ?? "Anonymous", 
                User.Identity?.IsAuthenticated ?? false,
                string.Join(", ", User.Claims.Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(c => c.Value)));
                
            try
            {
                var query = new GetAllRolesQuery();
                var result = await _mediator.Send(query);

                _logger.LogInformation("GetRoles returned {RoleCount} roles for user {Username}", 
                    result.Count, User.Identity?.Name ?? "Anonymous");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving roles for user {Username}", User.Identity?.Name ?? "Anonymous");
                return StatusCode(500, "Internal server error while retrieving roles");
            }
        }

        /// <summary>
        /// Get role by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationRoleDto>> GetRole(string id)
        {
            try
            {
                var query = new GetRoleByIdQuery(id);
                var result = await _mediator.Send(query);

                if (result == null)
                {
                    return NotFound($"Role with ID {id} not found");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving role {RoleId}", id);
                return StatusCode(500, "Internal server error while retrieving role");
            }
        }

        /// <summary>
        /// Create a new role
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var command = new CreateRoleCommand(request.Name);
                var result = await _mediator.Send(command);

                if (result.Succeeded)
                {
                    return Created($"/api/rolemanagement/{request.Name}", new { message = "Role created successfully" });
                }

                return BadRequest(new { message = result.FirstErrorMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role {RoleName}", request.Name);
                return StatusCode(500, "Internal server error while creating role");
            }
        }

        /// <summary>
        /// Update an existing role
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRole(string id, [FromBody] UpdateRoleRequest request)
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

                var command = new UpdateRoleCommand(request.Id, request.Name);
                var result = await _mediator.Send(command);

                if (result.Succeeded)
                {
                    return Ok(new { message = "Role updated successfully" });
                }

                return BadRequest(new { message = result.FirstErrorMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role {RoleId}", id);
                return StatusCode(500, "Internal server error while updating role");
            }
        }

        /// <summary>
        /// Delete a role
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRole(string id)
        {
            try
            {
                var command = new DeleteRoleCommand(id);
                var result = await _mediator.Send(command);

                if (result.Succeeded)
                {
                    return Ok(new { message = "Role deleted successfully" });
                }

                return BadRequest(new { message = result.FirstErrorMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role {RoleId}", id);
                return StatusCode(500, "Internal server error while deleting role");
            }
        }
    }
}