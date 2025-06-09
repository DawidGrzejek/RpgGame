using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Interfaces.World;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RpgGame.Application.Interfaces.Services
{
    /// <summary>
    /// Service for managing game saves, including saving, loading, and listing game saves
    /// </summary>
    public interface IGameSaveService
    {
        /// <summary>
        /// Starts a new game session and resets the play time tracking
        /// </summary>
        void StartNewSession();

        /// <summary>
        /// Updates the current play time based on session duration
        /// </summary>
        void UpdatePlayTime();

        /// <summary>
        /// Saves the current game state
        /// </summary>
        /// <param name="saveName">Name for the save file</param>
        /// <param name="player">The player character</param>
        /// <param name="currentLocation">The current location</param>
        /// <returns>True if save was successful</returns>
        Task<bool> SaveGameAsync(string saveName, Character player, ILocation currentLocation, CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves the current game state (synchronous version)
        /// </summary>
        bool SaveGame(string saveName, Character player, ILocation currentLocation);

        /// <summary>
        /// Loads a saved game
        /// </summary>
        /// <param name="saveName">Name of the save to load</param>
        /// <param name="gameWorld">Game world reference for location lookup</param>
        /// <param name="player">Output parameter for the loaded player</param>
        /// <param name="currentLocation">Output parameter for the loaded location</param>
        /// <returns>True if load was successful</returns>
        Task<(bool Success, Character Player, ILocation CurrentLocation)> LoadGameAsync(
            string saveName,
            IGameWorld gameWorld,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Loads a saved game (synchronous version)
        /// </summary>
        bool LoadGame(string saveName, IGameWorld gameWorld, out Character player, out ILocation currentLocation);

        /// <summary>
        /// Gets a list of all available save files
        /// </summary>
        /// <returns>List of save names with their save dates</returns>
        Task<List<(string SaveName, DateTime SaveDate)>> GetAvailableSavesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a list of all available save files (synchronous version)
        /// </summary>
        List<(string SaveName, DateTime SaveDate)> GetAvailableSaves();

        /// <summary>
        /// Deletes a saved game
        /// </summary>
        /// <param name="saveName">Name of the save to delete</param>
        /// <returns>True if deletion was successful</returns>
        Task<bool> DeleteSaveAsync(string saveName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a saved game (synchronous version)
        /// </summary>
        bool DeleteSave(string saveName);

        /// <summary>
        /// Gets the formatted play time string (e.g. "10h 15m 30s")
        /// </summary>
        /// <returns>Formatted play time string</returns>
        string GetFormattedPlayTime();
    }
}