using RpgGame.Application.Services;
using RpgGame.Domain.Events.Characters;
using RpgGame.Domain.Interfaces.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Application.Events.Handlers.Character
{
    /// <summary>
    /// Handles character state changed events for automatic saving
    /// </summary>
    public class CharacterStateChangedHandler : IEventHandler<CharacterLeveledUp>
    {
        private readonly GameSaveService _gameSaveService;
        private readonly IGameWorld _gameWorld;

        public CharacterStateChangedHandler(GameSaveService gameSaveService, IGameWorld gameWorld)
        {
            _gameSaveService = gameSaveService ?? throw new ArgumentNullException(nameof(gameSaveService));
            _gameWorld = gameWorld ?? throw new ArgumentNullException(nameof(gameWorld));
        }

        public async Task HandleAsync(CharacterLeveledUp domainEvent)
        {
            try
            {
                // Log the level up event
                Console.WriteLine($"Character '{domainEvent.CharacterName}' leveled up from {domainEvent.OldLevel} to {domainEvent.NewLevel}");

                // Here you would typically get the actual player character and current location
                // For now, we'll simulate this with a simplified approach

                // You might want to implement a character repository to get the character by name
                // var character = await _characterRepository.GetByNameAsync(domainEvent.CharacterName);
                // var currentLocation = _gameWorld.GetCurrentLocation(character);

                // Auto-save on level up
                // bool saveSuccess = _gameSaveService.SaveGame($"AutoSave_LevelUp_{domainEvent.NewLevel}", character, currentLocation);

                // For demonstration, we'll just log the intent
                Console.WriteLine($"Would auto-save character state after leveling up to level {domainEvent.NewLevel}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling CharacterLeveledUp event: {ex.Message}");
                // In a real application, you'd want proper error handling/logging here
            }
        }
    }
}
