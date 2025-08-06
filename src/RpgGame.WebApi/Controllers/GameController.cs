/*using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpgGame.Application.Commands.Game;
using RpgGame.Domain.Interfaces.World;
using RpgGame.WebApi.DTOs.Requests;

namespace RpgGame.WebApi.Controllers
{
    [ApiController]
    [Authorize(Policy = "GameMaster")]
    [Route("api/v{version:apiVersion}/game")]
    public class GameController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IGameWorld _gameWorld;

        public GameController(IMediator mediator, IGameWorld gameWorld)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _gameWorld = gameWorld ?? throw new ArgumentNullException(nameof(gameWorld));
        }

        [HttpPost("move")]
        public async Task<ActionResult> MoveToLocation([FromBody] MoveLocationRequest request)
        {
            var command = new MoveCharacterCommand
            {
                CharacterId = request.CharacterId,
                TargetLocation = request.LocationName
            };

            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(new { success = false, message = result.FirstErrorMessage });

            var location = _gameWorld.GetLocation(request.LocationName);

            return Ok(new
            {
                success = true,
                location = new
                {
                    name = location.Name,
                    description = location.Description,
                    connectedLocations = _gameWorld.GetConnectedLocations(location).Select(l => l.Name),
                    hasEnemies = location.PossibleEnemies.Count > 0,
                    canRest = IsLocationSafeForResting(location.Name)
                }
            });
        }

        [HttpPost("explore")]
        public async Task<ActionResult> ExploreLocation([FromBody] ExploreLocationRequest request)
        {
            var command = new ExploreLocationCommand
            {
                CharacterId = request.CharacterId,
                LocationName = request.LocationName
            };

            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(new { success = false, message = result.FirstErrorMessage });

            var exploreResult = result.Data;

            return Ok(new
            {
                success = true,
                enemyEncountered = exploreResult.EnemyEncountered,
                enemy = exploreResult.Enemy != null ? new
                {
                    id = exploreResult.Enemy.Id,
                    name = exploreResult.Enemy.Name,
                    health = exploreResult.Enemy.Health,
                    maxHealth = exploreResult.Enemy.MaxHealth,
                    strength = exploreResult.Enemy.Strength,
                    defense = exploreResult.Enemy.Defense,
                    experienceReward = exploreResult.Enemy.ExperienceReward,
                    isAlive = exploreResult.Enemy.IsAlive
                } : null,
                itemFound = exploreResult.ItemFound,
                itemName = exploreResult.ItemName,
                experienceGained = exploreResult.ExperienceGained,
                message = exploreResult.Message
            });
        }

        [HttpPost("random-encounter")]
        public async Task<ActionResult> TriggerRandomEncounter([FromBody] RandomEncounterRequest request)
        {
            var command = new RandomEncounterRequest
            {
                CharacterId = request.CharacterId,
                LocationName = request.LocationName
            };

            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(new { success = false, message = result.FirstErrorMessage });

            // Similar response structure as explore
            return Ok(result.Data);
        }

        [HttpPost("save")]
        public async Task<ActionResult> SaveGame([FromBody] SaveGameRequest request)
        {
            var command = new SaveGameRequest
            {
                CharacterId = request.CharacterId,
                LocationName = request.LocationName,
                SaveName = request.SaveName
            };

            var result = await _mediator.Send(command);

            return Ok(new
            {
                success = result.Succeeded,
                message = result.Succeeded ? "Game saved successfully" : result.FirstErrorMessage
            });
        }

        private bool IsLocationSafeForResting(string locationName)
        {
            return locationName.ToLower() == "town" || locationName.ToLower().Contains("safe");
        }
    }
}
*/