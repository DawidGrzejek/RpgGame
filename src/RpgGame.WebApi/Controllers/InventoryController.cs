using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RpgGame.Application.Commands;
using RpgGame.Application.Commands.Inventory;
using RpgGame.Application.Queries.Inventory;
using RpgGame.Application.Serialization.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RpgGame.WebApi.Controllers
{
    /// <summary>
    /// Controller for managing character inventory operations.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/characters/{characterId}/inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator instance for handling commands and queries.</param>
        /// <param name="mapper">The mapper instance for mapping between DTOs and domain models.</param>
        public InventoryController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves the inventory of a specific character.
        /// </summary>
        /// <param name="characterId">The unique identifier of the character.</param>
        /// <returns>A list of items in the character's inventory.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetInventory(Guid characterId)
        {
            var query = new GetCharacterInventoryQuery { CharacterId = characterId };
            var result = await _mediator.Send(query);

            if (!result.Succeeded)
                return NotFound(result.Message);

            return Ok(_mapper.Map<IEnumerable<ItemDto>>(result.Data));
        }

        /// <summary>
        /// Equips an item for a specific character.
        /// </summary>
        /// <param name="characterId">The unique identifier of the character.</param>
        /// <param name="itemId">The unique identifier of the item to equip.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        [HttpPost("equip/{itemId}")]
        public async Task<ActionResult> EquipItem(Guid characterId, Guid itemId)
        {
            var command = new EquipItemCommand
            {
                CharacterId = characterId,
                ItemId = itemId
            };

            var result = await _mediator.Send<CommandResult>(command);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        /// <summary>
        /// Uses an item from the inventory of a specific character.
        /// </summary>
        /// <param name="characterId">The unique identifier of the character.</param>
        /// <param name="itemId">The unique identifier of the item to use.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        [HttpPost("use/{itemId}")]
        public async Task<ActionResult> UseItem(Guid characterId, Guid itemId)
        {
            var command = new UseItemCommand
            {
                CharacterId = characterId,
                ItemId = itemId
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }
    }
}