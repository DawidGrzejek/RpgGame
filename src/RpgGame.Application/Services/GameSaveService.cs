using RpgGame.Application.Events;
using RpgGame.Application.Interfaces.Repositories;
using RpgGame.Application.Interfaces.Services;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Events.Game;
using RpgGame.Domain.Interfaces.World;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RpgGame.Application.Services
{
    /// <summary>
    /// Service for managing game saves, including saving, loading, and listing game saves
    /// </summary>
    public class GameSaveService : IGameSaveService, IDisposable
    {
        private readonly IGameSaveRepository _saveRepository;
        private readonly IEventStoreRepository _eventStore;
        private int _currentPlayTime; // Track play time in seconds
        private DateTime _sessionStartTime;
        private bool _disposed = false;

        public GameSaveService(IGameSaveRepository saveRepository, IEventStoreRepository eventStore)
        {
            _saveRepository = saveRepository ?? throw new ArgumentNullException(nameof(saveRepository));
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _currentPlayTime = 0;
            _sessionStartTime = DateTime.Now;
        }

        /// <summary>
        /// Starts a new game session and resets the play time tracking
        /// </summary>
        public void StartNewSession()
        {
            _sessionStartTime = DateTime.Now;
            _currentPlayTime = 0;
        }

        /// <summary>
        /// Updates the current play time based on session duration
        /// </summary>
        public void UpdatePlayTime()
        {
            TimeSpan sessionDuration = DateTime.Now - _sessionStartTime;
            _currentPlayTime += (int)sessionDuration.TotalSeconds;
            _sessionStartTime = DateTime.Now; // Reset for next update
        }

        /// <summary>
        /// Saves the current game state
        /// </summary>
        /// <param name="saveName">Name for the save file</param>
        /// <param name="player">The player character</param>
        /// <param name="currentLocation">The current location</param>
        /// <returns>True if save was successful</returns>
        public async Task<bool> SaveGameAsync(
            string saveName,
            Character player,
            ILocation currentLocation,
            CancellationToken cancellationToken = default)
        {
            // Update play time before saving
            UpdatePlayTime();

            // Also store a game save event
            var saveEvent = new GameSavedEvent(
                Guid.NewGuid(), // Or use a consistent ID for the game
                1, // Version
                saveName,
                player.Name,
                player.Level,
                currentLocation.Name,
                DateTime.UtcNow
            );

            await _eventStore.SaveEventAsync(saveEvent, cancellationToken: cancellationToken);

            return await _saveRepository.SaveGameAsync(
                saveName,
                player,
                currentLocation.Name,
                _currentPlayTime,
                cancellationToken);
        }

        /// <summary>
        /// Synchronous version of SaveGameAsync
        /// </summary>
        public bool SaveGame(string saveName, Character player, ILocation currentLocation)
        {
            return SaveGameAsync(saveName, player, currentLocation).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Loads a saved game
        /// </summary>
        /// <param name="saveName">Name of the save to load</param>
        /// <param name="gameWorld">Game world reference for location lookup</param>
        /// <returns>Tuple containing success flag, player character, and current location</returns>
        public async Task<(bool Success, Character Player, ILocation CurrentLocation)> LoadGameAsync(
            string saveName,
            IGameWorld gameWorld,
            CancellationToken cancellationToken = default)
        {
            var result = await _saveRepository.LoadGameAsync(saveName, cancellationToken);

            if (!result.Success)
            {
                return (false, null, null);
            }

            // Get the location from the game world
            ILocation currentLocation = gameWorld.GetLocation(result.CurrentLocationName);
            if (currentLocation == null)
            {
                // If location not found, use starting location
                Console.WriteLine($"Location '{result.CurrentLocationName}' not found. Using starting location.");
                currentLocation = gameWorld.StartLocation;
            }

            // Set play time
            _currentPlayTime = result.PlayTime;
            _sessionStartTime = DateTime.Now; // Reset session start time

            return (true, result.PlayerCharacter, currentLocation);
        }

        /// <summary>
        /// Synchronous version of LoadGameAsync
        /// </summary>
        public bool LoadGame(
            string saveName,
            IGameWorld gameWorld,
            out Character player,
            out ILocation currentLocation)
        {
            var result = LoadGameAsync(saveName, gameWorld).GetAwaiter().GetResult();

            player = result.Player;
            currentLocation = result.CurrentLocation;

            return result.Success;
        }

        /// <summary>
        /// Gets a list of all available save files
        /// </summary>
        /// <returns>List of save names with their save dates</returns>
        public async Task<List<(string SaveName, DateTime SaveDate)>> GetAvailableSavesAsync(
            CancellationToken cancellationToken = default)
        {
            return await _saveRepository.GetAllSavesAsync(cancellationToken);
        }

        /// <summary>
        /// Synchronous version of GetAvailableSavesAsync
        /// </summary>
        public List<(string SaveName, DateTime SaveDate)> GetAvailableSaves()
        {
            return GetAvailableSavesAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deletes a saved game
        /// </summary>
        /// <param name="saveName">Name of the save to delete</param>
        /// <returns>True if deletion was successful</returns>
        public async Task<bool> DeleteSaveAsync(string saveName, CancellationToken cancellationToken = default)
        {
            return await _saveRepository.DeleteSaveAsync(saveName, cancellationToken);
        }

        /// <summary>
        /// Synchronous version of DeleteSaveAsync
        /// </summary>
        public bool DeleteSave(string saveName)
        {
            return DeleteSaveAsync(saveName).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the formatted play time string (e.g. "10h 15m 30s")
        /// </summary>
        /// <returns>Formatted play time string</returns>
        public string GetFormattedPlayTime()
        {
            // Update current play time
            UpdatePlayTime();

            // Calculate hours, minutes, seconds
            int totalSeconds = _currentPlayTime;
            int hours = totalSeconds / 3600;
            totalSeconds %= 3600;
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            // Format the string
            if (hours > 0)
            {
                return $"{hours}h {minutes}m {seconds}s";
            }
            else if (minutes > 0)
            {
                return $"{minutes}m {seconds}s";
            }
            else
            {
                return $"{seconds}s";
            }
        }

        #region IDisposable Implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose any owned disposable resources
                    (_saveRepository as IDisposable)?.Dispose();
                }

                _disposed = true;
            }
        }

        ~GameSaveService()
        {
            Dispose(false);
        }

        #endregion
    }
}