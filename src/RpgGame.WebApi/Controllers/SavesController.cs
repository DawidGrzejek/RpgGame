using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpgGame.Application.Interfaces.Services;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Interfaces.World;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RpgGame.WebApi.Controllers
{
    [ApiController]
    [Authorize(Policy = "GameMaster")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SavesController : ControllerBase
    {
        private readonly IGameSaveService _gameSaveService;
        private readonly IGameWorld _gameWorld;

        public SavesController(IGameSaveService gameSaveService, IGameWorld gameWorld)
        {
            _gameSaveService = gameSaveService ?? throw new ArgumentNullException(nameof(gameSaveService));
            _gameWorld = gameWorld ?? throw new ArgumentNullException(nameof(gameWorld));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaveFileDto>>> GetAllSavesAsync()
        {
            var saves = await _gameSaveService.GetAvailableSavesAsync();
            var result = new List<SaveFileDto>();

            foreach (var (name, date) in saves)
            {
                result.Add(new SaveFileDto
                {
                    SaveName = name,
                    SaveDate = date,
                    SavedAt = date.ToString("g")
                });
            }

            return Ok(result);
        }

        [HttpGet("{saveName}")]
        public async Task<ActionResult<SaveDetailsDto>> GetSaveDetailsAsync(string saveName)
        {
            // In a full implementation, you would load additional details about the save
            // For now, we'll just check if the save exists
            var saves = await _gameSaveService.GetAvailableSavesAsync();
            var saveInfo = saves.Find(s => s.SaveName == saveName);

            if (saveInfo == default)
                return NotFound($"Save '{saveName}' not found");

            var loadResult = await _gameSaveService.LoadGameAsync(saveName, _gameWorld);
            if (!loadResult.Success)
                return NotFound($"Failed to load save '{saveName}'");

            return Ok(new SaveDetailsDto
            {
                SaveName = saveName,
                SaveDate = saveInfo.SaveDate,
                SavedAt = saveInfo.SaveDate.ToString("g"),
                CharacterName = loadResult.Player.Name,
                CharacterLevel = loadResult.Player.Level,
                LocationName = loadResult.CurrentLocation.Name,
                PlayTime = _gameSaveService.GetFormattedPlayTime()
            });
        }

        [HttpPost]
        public async Task<ActionResult> SaveGameAsync(SaveGameRequest request)
        {
            if (request == null)
                return BadRequest("Invalid save game request");

            if (string.IsNullOrWhiteSpace(request.SaveName))
                return BadRequest("Save name cannot be empty");

            // In a real implementation, you would get the player and location from the session
            // For demonstration, we'll return a 501 Not Implemented
            return StatusCode(501, "Save Game API is not fully implemented yet");
        }

        [HttpDelete("{saveName}")]
        public async Task<ActionResult> DeleteSaveAsync(string saveName)
        {
            if (string.IsNullOrWhiteSpace(saveName))
                return BadRequest("Save name cannot be empty");

            bool result = await _gameSaveService.DeleteSaveAsync(saveName);
            if (!result)
                return NotFound($"Save '{saveName}' not found or could not be deleted");

            return NoContent();
        }
    }

    public class SaveFileDto
    {
        public string? SaveName { get; set; }
        public DateTime SaveDate { get; set; }
        public string? SavedAt { get; set; }
    }

    public class SaveDetailsDto
    {
        public string? SaveName { get; set; }
        public DateTime SaveDate { get; set; }
        public string? SavedAt { get; set; }
        public string? CharacterName { get; set; }
        public int CharacterLevel { get; set; }
        public string? LocationName { get; set; }
        public string? PlayTime { get; set; }
    }

    public class SaveGameRequest
    {
        public string? SaveName { get; set; }
    }
}