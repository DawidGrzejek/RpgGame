using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RpgGame.Application.Interfaces.Persistence;
using RpgGame.Application.Interfaces.Repositories;

namespace RpgGame.WebApi.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserRepository _userRepository;
        private IUnitOfWork _unitOfWork;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IMediator mediator,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ILogger<UsersController> logger)
        {
            _mediator = mediator;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpPost("{userId}/characters")]
        public async Task<IActionResult> AssignCharacterToUser(Guid userId, [FromBody] AssignCharacterRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", userId);
                return NotFound(new { Message = "User not found" });
            }

            try
            {
                user.AddCharacter(request.CharacterId, request.CharacterName, request.CharacterType);

                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Character {CharacterName} assigned to user {UserId}", request.CharacterName, userId);
                return Ok(new { Message = "Character assigned successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning character to user {UserId}", userId);
                return StatusCode(500, new { Message = "An error occurred while assigning the character" });
            }
        }

        [HttpPost("{userId}/roles")]
        public async Task<IActionResult> AddRoleToUser(Guid userId, [FromBody] AddRoleRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return NotFound();

            // Domain logic - this will raise UserRoleAddedEvent
            user.AddRole(request.Role);

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{userId}/deactivate")]
        public async Task<IActionResult> DeactivateUser(Guid userId, [FromBody] DeactivateUserRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return NotFound();

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";

            // Domain logic - this will raise UserDeactivatedEvent
            user.Deactivate(request.Reason, currentUserId, request.IsTemporary, request.ReactivationDate);

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return Ok();
        }
    }

    public record AssignCharacterRequest(Guid CharacterId, string CharacterName, string CharacterType);
    public record AddRoleRequest(string Role);
    public record DeactivateUserRequest(string Reason, bool IsTemporary = false, DateTime? ReactivationDate = null);
}