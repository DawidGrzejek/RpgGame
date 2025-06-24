using MediatR;
using Microsoft.AspNetCore.Mvc;
using RpgGame.Application.Commands;
using RpgGame.Application.Commands.Combat;
using RpgGame.Application.Commands.Game;
using RpgGame.Application.Commands.Results;
using RpgGame.Domain.Common;
using RpgGame.WebApi.DTOs.Requests;

namespace RpgGame.WebApi.Controllers
{
    /// <summary>
    /// Controller for managing combat operations in the RPG game.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/combat")]
    public class CombatController : ControllerBase
    {
        private readonly IMediator _mediator;


        /// <summary>
        /// Initializes a new instance of the <see cref="CombatController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator instance for handling commands and queries.</param>
        /// <param name="mapper">The mapper instance for mapping between DTOs and domain models.</param>
        public CombatController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("attack")]
        public async Task<ActionResult> PerformAttack([FromBody] AttackRequest request)
        {
            var command = new PerformAttackCommand
            {
                AttackerId = request.AttackerId,
                DefenderId = request.DefenderId
            };

            //var result = await _mediator.Send<OperationResult<CombatResult>>(command);
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(new { message = result.FirstErrorMessage });

            var combatResult = result.Data;

            return Ok(new
            {
                playerDamage = combatResult.PlayerDamage,
                enemyDamage = combatResult.EnemyDamage,
                playerHealth = combatResult.PlayerHealth,
                enemyHealth = combatResult.EnemyHealth,
                enemyDefeated = combatResult.EnemyDefeated,
                playerDefeated = combatResult.PlayerDefeated,
                experienceGained = combatResult.ExperienceGained,
                itemsDropped = combatResult.ItemsDropped
            });
        }

        [HttpPost("flee")]
        public async Task<ActionResult> FleeCombat([FromBody] FleeRequest request)
        {
            var command = new FleeCombatCommand
            {
                CharacterId = request.CharacterId,
                EnemyId = request.EnemyId
            };

            var result = await _mediator.Send(command);

            return Ok(new
            {
                success = result.Succeeded,
                message = result.Succeeded ? "Successfully fled from combat!" : result.FirstErrorMessage,
                enemyAttack = result.Data?.EnemyCounterAttack
            });
        }
    }
}
