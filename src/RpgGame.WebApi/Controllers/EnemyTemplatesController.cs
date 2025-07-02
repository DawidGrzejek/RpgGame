using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RpgGame.Application.Commands.EntityTemplates;
using RpgGame.Application.Queries.EntityTemplates;
using RpgGame.Domain.Enums;
using RpgGame.WebApi.DTOs.Requests;
using RpgGame.WebApi.DTOs.Responses;

namespace RpgGame.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/enemy-templates")]
    public class EnemyTemplatesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EnemyTemplatesController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnemyTemplateResponse>>> GetEnemyTemplates()
        {
            var query = new GetAllEnemyTemplatesQuery();
            var templates = await _mediator.Send(query);
            
            var response = templates.Select(t => new EnemyTemplateResponse
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                BaseHealth = t.BaseHealth,
                BaseStrength = t.BaseStrength,
                BaseDefense = t.BaseDefense,
                ExperienceReward = t.ExperienceReward,
                EnemyType = t.EnemyType.ToString(),
                PossibleLoot = t.PossibleLoot.ToList(),
                SpecialAbilities = t.SpecialAbilities
            });

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EnemyTemplateResponse>> GetEnemyTemplate(Guid id)
        {
            var query = new GetEnemyTemplateByIdQuery(id);
            var template = await _mediator.Send(query);

            if (template == null)
                return NotFound();

            var response = new EnemyTemplateResponse
            {
                Id = template.Id,
                Name = template.Name,
                Description = template.Description,
                BaseHealth = template.BaseHealth,
                BaseStrength = template.BaseStrength,
                BaseDefense = template.BaseDefense,
                ExperienceReward = template.ExperienceReward,
                EnemyType = template.EnemyType.ToString(),
                PossibleLoot = template.PossibleLoot.ToList(),
                SpecialAbilities = template.SpecialAbilities
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<EnemyTemplateResponse>> CreateEnemyTemplate([FromBody] CreateEnemyTemplateRequest request)
        {
            if (!Enum.TryParse<EnemyType>(request.EnemyType, out var enemyType))
                return BadRequest("Invalid enemy type");

            var command = new CreateEnemyTemplateCommand(
                request.Name,
                request.Description,
                request.BaseHealth,
                request.BaseStrength,
                request.BaseDefense,
                request.ExperienceReward,
                enemyType,
                request.PossibleLoot,
                request.SpecialAbilities
            );

            var template = await _mediator.Send(command);

            var response = new EnemyTemplateResponse
            {
                Id = template.Id,
                Name = template.Name,
                Description = template.Description,
                BaseHealth = template.BaseHealth,
                BaseStrength = template.BaseStrength,
                BaseDefense = template.BaseDefense,
                ExperienceReward = template.ExperienceReward,
                EnemyType = template.EnemyType.ToString(),
                PossibleLoot = template.PossibleLoot.ToList(),
                SpecialAbilities = template.SpecialAbilities
            };

            return CreatedAtAction(nameof(GetEnemyTemplate), new { id = template.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EnemyTemplateResponse>> UpdateEnemyTemplate(Guid id, [FromBody] UpdateEnemyTemplateRequest request)
        {
            if (!Enum.TryParse<EnemyType>(request.EnemyType, out var enemyType))
                return BadRequest("Invalid enemy type");

            var command = new UpdateEnemyTemplateCommand(
                id,
                request.Name,
                request.Description,
                request.BaseHealth,
                request.BaseStrength,
                request.BaseDefense,
                request.ExperienceReward,
                enemyType,
                request.PossibleLoot,
                request.SpecialAbilities
            );

            var template = await _mediator.Send(command);

            if (template == null)
                return NotFound();

            var response = new EnemyTemplateResponse
            {
                Id = template.Id,
                Name = template.Name,
                Description = template.Description,
                BaseHealth = template.BaseHealth,
                BaseStrength = template.BaseStrength,
                BaseDefense = template.BaseDefense,
                ExperienceReward = template.ExperienceReward,
                EnemyType = template.EnemyType.ToString(),
                PossibleLoot = template.PossibleLoot.ToList(),
                SpecialAbilities = template.SpecialAbilities
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEnemyTemplate(Guid id)
        {
            var command = new DeleteEnemyTemplateCommand(id);
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}