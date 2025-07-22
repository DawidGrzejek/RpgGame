using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Interfaces.World;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RpgGame.Application.Interfaces.Repositories
{
    /// <summary>
    /// Repository for game save operations
    /// </summary>
    public interface IGameSaveRepository
    {
        /// <summary>
        /// Saves the game state to the repository
        /// </summary>
        /// <param name="saveName">Name of the save</param>
        /// <param name="playerCharacter">Player character to save</param>
        /// <param name="currentLocationName">Current location name</param>
        /// <param name="playTime">Current play time in seconds</param>
        /// <returns>True if the save was successful</returns>
        Task<bool> SaveGameAsync(
            string saveName,
            Character playerCharacter,
            string currentLocationName,
            int playTime,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Synchronous version of SaveGameAsync
        /// </summary>
        bool SaveGame(string saveName, Character playerCharacter, string currentLocationName, int playTime);

        /// <summary>
        /// Loads a game save from the repository
        /// </summary>
        /// <param name="saveName">Name of the save to load</param>
        /// <param name="gameWorld">Game world reference for location resolution</param>
        /// <returns>A tuple with success flag, player character, current location, and play time</returns>
        Task<(bool Success, Character PlayerCharacter, string CurrentLocationName, int PlayTime)> LoadGameAsync(
            string saveName,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Synchronous version of LoadGameAsync
        /// </summary>
        bool LoadGame(string saveName, out Character playerCharacter, out string currentLocationName, out int playTime);

        /// <summary>
        /// Gets all available saves from the repository
        /// </summary>
        /// <returns>List of save names with their save dates</returns>
        Task<List<(string SaveName, DateTime SaveDate)>> GetAllSavesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Synchronous version of GetAllSavesAsync
        /// </summary>
        List<(string SaveName, DateTime SaveDate)> GetAllSaves();

        /// <summary>
        /// Deletes a save from the repository
        /// </summary>
        /// <param name="saveName">Name of the save to delete</param>
        /// <returns>True if the deletion was successful</returns>
        Task<bool> DeleteSaveAsync(string saveName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Synchronous version of DeleteSaveAsync
        /// </summary>
        bool DeleteSave(string saveName);
    }
}