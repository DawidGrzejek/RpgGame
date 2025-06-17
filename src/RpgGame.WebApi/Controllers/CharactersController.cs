using Asp.Versioning;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RpgGame.Application.Commands;
using RpgGame.Application.Commands.Characters;
using RpgGame.Application.Interfaces.Services;
using RpgGame.Application.Queries.Characters;
using RpgGame.WebApi.DTOs.Characters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RpgGame.WebApi.Controllers
{
    /// <summary>
    /// Controller for managing character-related operations in the RPG game.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CharactersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<CharactersController> _logger;
        private readonly ICharacterService _characterService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharactersController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator instance used for handling requests and commands.</param>
        /// <param name="mapper">The mapper instance used for mapping objects between models and DTOs.</param>
        /// <param name="logger">The logger instance used for logging application events and errors.</param>
        /// <param name="characterService">The service instance used for managing character-related operations.</param>
        /// <exception cref="ArgumentNullException">Thrown if any of the parameters <paramref name="mediator"/>, <paramref name="mapper"/>, <paramref
        /// name="logger"/>, or <paramref name="characterService"/> is <see langword="null"/>.</exception>
        public CharactersController(
            IMediator mediator,
            IMapper mapper,
            ILogger<CharactersController> logger,
            ICharacterService characterService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _characterService = characterService ?? throw new ArgumentNullException(nameof(characterService));
        }

        /// <summary>
        /// Retrieves all characters in the game.
        /// </summary>
        /// <returns>A list of character summaries.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CharacterSummaryDto>>> GetAllCharacters()
        {
            var query = new GetAllCharactersQuery();
            var result = await _mediator.Send(query);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(_mapper.Map<IEnumerable<CharacterSummaryDto>>(result.Data));
        }

        /// <summary>
        /// Retrieves a specific character by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the character.</param>
        /// <returns>The details of the character.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CharacterDto>> GetCharacter(Guid id)
        {
            var query = new GetCharacterByIdQuery { CharacterId = id };
            var result = await _mediator.Send(query);

            if (!result.Success)
                return NotFound(result.Message);

            return Ok(_mapper.Map<CharacterDto>(result.Data));
        }

        /// <summary>
        /// Creates a new character.
        /// </summary>
        /// <param name="createCharacterDto">The data for creating the character.</param>
        /// <returns>The created character's details.</returns>
        [HttpPost]
        public async Task<ActionResult<CharacterDto>> CreateCharacter(CreateCharacterDto createCharacterDto)
        {
            var command = _mapper.Map<CreateCharacterCommand>(createCharacterDto);
            CommandResult result = await _mediator.Send<CommandResult>(command);

            if (!result.Success)
                return BadRequest(result.Message);

            var characterDto = _mapper.Map<CharacterDto>(result.Data);
            return CreatedAtAction(nameof(GetCharacter), new { id = characterDto.Id }, characterDto);
        }

        /// <summary>
        /// Levels up a specific character.
        /// </summary>
        /// <param name="id">The unique identifier of the character to level up.</param>
        /// <returns>A success message if the operation is successful.</returns>
        [HttpPost("{id}/levelup")]
        public async Task<ActionResult> LevelUp(Guid id)
        {
            var command = new LevelUpCharacterCommand { CharacterId = id };
            CommandResult result = await _mediator.Send<CommandResult>(command);

            if (!result.Success)
                return NotFound(result.Message);

            return Ok(result.Message);
        }

        /// <summary>
        /// Retrieves the history of a specific character.
        /// </summary>
        /// <param name="id">The unique identifier of the character.</param>
        /// <returns>A list of historical events related to the character.</returns>
        [HttpGet("{id}/history")]
        public async Task<ActionResult> GetCharacterHistory(Guid id)
        {
            var query = new GetCharacterHistoryQuery { CharacterId = id };
            var result = await _mediator.Send(query);

            if (!result.Success)
                return NotFound(result.Message);

            // Transform events to a more user-friendly format
            var historyItems = new List<object>();
            foreach (var @event in result.Data)
            {
                historyItems.Add(new
                {
                    EventType = @event.EventType,
                    Timestamp = @event.OccurredAt,
                    Details = GetEventDetails(@event)
                });
            }

            return Ok(historyItems);
        }

        /// <summary>
        /// Extracts relevant details from a character event.
        /// </summary>
        /// <param name="event">The event to extract details from.</param>
        /// <returns>An object containing the event details.</returns>
        private object GetEventDetails(dynamic @event)
        {
            string eventType = @event.EventType;

            switch (eventType)
            {
                case "CharacterCreatedEvent":
                    return new
                    {
                        Name = @event.Name,
                        Type = @event.CharacterType.ToString()
                    };
                case "CharacterLeveledUp":
                    return new
                    {
                        OldLevel = @event.OldLevel,
                        NewLevel = @event.NewLevel,
                        HealthIncrease = @event.HealthIncrease,
                        StrengthIncrease = @event.StrengthIncrease,
                        DefenseIncrease = @event.DefenseIncrease
                    };
                case "CharacterDied":
                    return new
                    {
                        Location = @event.Location
                    };
                default:
                    return new { };
            }
        }
    }
}