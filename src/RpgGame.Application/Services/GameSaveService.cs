using RpgGame.Application.Events;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Events.Game;
using RpgGame.Domain.Interfaces.World;
using RpgGame.Infrastructure.Persistence.EFCore;
using System;
using System.Collections.Generic;

namespace RpgGame.Application.Services
{
    /// <summary>
    /// Service for managing game saves, including saving, loading, and listing game saves
    /// </summary>
    public class GameSaveService : IDisposable
    {
        private readonly EfGameSaveRepository _saveRepository;
        private int _currentPlayTime; // Track play time in seconds
        private DateTime _sessionStartTime;
        private bool _disposed = false;
        private readonly IEventStoreRepository _eventStore;

        public GameSaveService(IEventStoreRepository eventStore)
        {
            _eventStore = eventStore;
            _saveRepository = new EfGameSaveRepository();
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
        public bool SaveGame(string saveName, Character player, ILocation currentLocation)
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

            _eventStore.SaveEventAsync(saveEvent).GetAwaiter().GetResult();

            return _saveRepository.SaveGame(saveName, player, currentLocation, _currentPlayTime);
        }

        /// <summary>
        /// Loads a saved game
        /// </summary>
        /// <param name="saveName">Name of the save to load</param>
        /// <param name="gameWorld">Game world reference for location lookup</param>
        /// <param name="player">Output parameter for the loaded player</param>
        /// <param name="currentLocation">Output parameter for the loaded location</param>
        /// <returns>True if load was successful</returns>
        public bool LoadGame(string saveName, IGameWorld gameWorld, out Character player, out ILocation currentLocation)
        {
            bool result = _saveRepository.LoadGame(saveName, gameWorld, out player, out currentLocation, out int playTime);

            if (result)
            {
                _currentPlayTime = playTime;
                _sessionStartTime = DateTime.Now; // Reset session start time
            }

            return result;
        }

        /// <summary>
        /// Gets a list of all available save files
        /// </summary>
        /// <returns>List of save names with their save dates</returns>
        public List<(string SaveName, DateTime SaveDate)> GetAvailableSaves()
        {
            return _saveRepository.GetAllSaves();
        }

        /// <summary>
        /// Deletes a saved game
        /// </summary>
        /// <param name="saveName">Name of the save to delete</param>
        /// <returns>True if deletion was successful</returns>
        public bool DeleteSave(string saveName)
        {
            return _saveRepository.DeleteSave(saveName);
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

        /// <summary>
        /// Disposes the repository
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the repository
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _saveRepository?.Dispose();
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~GameSaveService()
        {
            Dispose(false);
        }
    }
}