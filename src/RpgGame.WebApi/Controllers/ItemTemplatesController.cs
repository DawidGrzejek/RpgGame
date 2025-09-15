using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpgGame.Application.Commands.EntityTemplates;
using RpgGame.Application.Queries.EntityTemplates;
using RpgGame.Domain.Enums;
using RpgGame.WebApi.DTOs.Requests;
using RpgGame.WebApi.DTOs.Responses;

namespace RpgGame.WebApi.Controllers
{
    [ApiController]
    [Authorize(Policy = "GameMaster")]
    [Route("api/v{version:apiVersion}/item-templates")]
    public class ItemTemplatesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ItemTemplatesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemTemplateResponse>>> GetItemTemplates()
        {
            var query = new GetAllItemTemplatesQuery();
            var templates = await _mediator.Send(query);
            
            var response = templates.Select(t => new ItemTemplateResponse
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                ItemType = t.ItemType.ToString(),
                Value = t.Value,
                StatModifiers = t.StatModifiers,
                IsConsumable = t.IsConsumable,
                IsEquippable = t.IsEquippable,
                EquipmentSlot = t.EquipmentSlot?.ToString()
            });

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ItemTemplateResponse>> CreateItemTemplate([FromBody] CreateItemTemplateRequest request)
        {
            if (!Enum.TryParse<ItemType>(request.ItemType, out var itemType))
                return BadRequest("Invalid item type");

            EquipmentSlot? equipmentSlot = null;
            if (!string.IsNullOrEmpty(request.EquipmentSlot))
            {
                if (!Enum.TryParse<EquipmentSlot>(request.EquipmentSlot, out var slot))
                    return BadRequest("Invalid equipment slot");
                equipmentSlot = slot;
            }

            var command = new CreateItemTemplateCommand(
                request.Name,
                request.Description,
                itemType,
                request.Value,
                request.StatModifiers,
                request.IsConsumable,
                request.IsEquippable,
                equipmentSlot
            );

            var template = await _mediator.Send(command);

            var response = new ItemTemplateResponse
            {
                Id = template.Id,
                Name = template.Name,
                Description = template.Description,
                ItemType = template.ItemType.ToString(),
                Value = template.Value,
                StatModifiers = template.StatModifiers,
                IsConsumable = template.IsConsumable,
                IsEquippable = template.IsEquippable,
                EquipmentSlot = template.EquipmentSlot?.ToString()
            };

            return CreatedAtAction(nameof(GetItemTemplate), new { id = template.Id }, response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemTemplateResponse>> GetItemTemplate(Guid id)
        {
            var query = new GetItemTemplateByIdQuery(id);
            var template = await _mediator.Send(query);

            if (template == null)
                return NotFound();

            var response = new ItemTemplateResponse
            {
                Id = template.Id,
                Name = template.Name,
                Description = template.Description,
                ItemType = template.ItemType.ToString(),
                Value = template.Value,
                StatModifiers = template.StatModifiers,
                IsConsumable = template.IsConsumable,
                IsEquippable = template.IsEquippable,
                EquipmentSlot = template.EquipmentSlot?.ToString()
            };

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ItemTemplateResponse>> UpdateItemTemplate(Guid id, [FromBody] UpdateItemTemplateRequest request)
        {
            if (!Enum.TryParse<ItemType>(request.ItemType, out var itemType))
                return BadRequest("Invalid item type");

            EquipmentSlot? equipmentSlot = null;
            if (!string.IsNullOrEmpty(request.EquipmentSlot))
            {
                if (!Enum.TryParse<EquipmentSlot>(request.EquipmentSlot, out var slot))
                    return BadRequest("Invalid equipment slot");
                equipmentSlot = slot;
            }

            var command = new UpdateItemTemplateCommand(
                id,
                request.Name,
                request.Description,
                itemType,
                request.Value,
                request.StatModifiers,
                request.IsConsumable,
                request.IsEquippable,
                equipmentSlot
            );

            var template = await _mediator.Send(command);

            if (template == null)
                return NotFound();

            var response = new ItemTemplateResponse
            {
                Id = template.Id,
                Name = template.Name,
                Description = template.Description,
                ItemType = template.ItemType.ToString(),
                Value = template.Value,
                StatModifiers = template.StatModifiers,
                IsConsumable = template.IsConsumable,
                IsEquippable = template.IsEquippable,
                EquipmentSlot = template.EquipmentSlot?.ToString()
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteItemTemplate(Guid id)
        {
            var command = new DeleteItemTemplateCommand(id);
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}