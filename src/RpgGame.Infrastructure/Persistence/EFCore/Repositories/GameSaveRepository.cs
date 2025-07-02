using Microsoft.EntityFrameworkCore;
using RpgGame.Application.Repositories;
using RpgGame.Application.Serialization.DTOs;
using RpgGame.Application.Serialization.Mappers;
using RpgGame.Domain.Entities.Characters.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RpgGame.Infrastructure.Persistence.EFCore
{
    public class GameSaveRepository : IGameSaveRepository, IDisposable
    {
        private readonly GameDbContext _context;
        private bool _disposed = false;

        public GameSaveRepository(GameDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Async Methods

        public async Task<bool> SaveGameAsync(
            string saveName,
            Character playerCharacter,
            string currentLocationName,
            int playTime,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var existingSave = await _context.GameSaves
                    .FirstOrDefaultAsync(g => g.SaveName == saveName, cancellationToken);

                if (existingSave != null)
                {
                    // Update existing save
                    existingSave.PlayerCharacter = playerCharacter;
                    existingSave.CurrentLocationName = currentLocationName;
                    existingSave.PlayTime = playTime;
                    existingSave.CreatedAt = DateTime.Now;
                }
                else
                {
                    // Create new save
                    await _context.GameSaves.AddAsync(new GameSave
                    {
                        SaveName = saveName,
                        PlayerCharacter = playerCharacter,
                        CurrentLocationName = currentLocationName,
                        PlayTime = playTime,
                        CreatedAt = DateTime.Now
                    }, cancellationToken);
                }

                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error saving game: {ex.Message}");
                return false;
            }
        }

        public async Task<(bool Success, Character PlayerCharacter, string CurrentLocationName, int PlayTime)> LoadGameAsync(
            string saveName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var gameSave = await _context.GameSaves
                    .FirstOrDefaultAsync(g => g.SaveName == saveName, cancellationToken);

                if (gameSave == null)
                {
                    Console.WriteLine($"No save found with name: {saveName}");
                    return (false, null, null, 0);
                }

                // Get the player character from the save
                var playerCharacter = gameSave.PlayerCharacter;
                var currentLocationName = gameSave.CurrentLocationName;
                var playTime = gameSave.PlayTime;

                return (true, playerCharacter, currentLocationName, playTime);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading game: {ex.Message}");
                return (false, null, null, 0);
            }
        }

        public async Task<List<(string SaveName, DateTime SaveDate)>> GetAllSavesAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.GameSaves
                    .OrderByDescending(g => g.CreatedAt)
                    .Select(g => new { g.SaveName, g.CreatedAt })
                    .AsNoTracking()
                    .ToListAsync(cancellationToken)
                    .ContinueWith(t => t.Result.Select(r => (r.SaveName, r.CreatedAt)).ToList());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting saves: {ex.Message}");
                return new List<(string, DateTime)>();
            }
        }

        public async Task<bool> DeleteSaveAsync(string saveName, CancellationToken cancellationToken = default)
        {
            try
            {
                var gameSave = await _context.GameSaves
                    .FirstOrDefaultAsync(g => g.SaveName == saveName, cancellationToken);

                if (gameSave == null)
                    return false;

                _context.GameSaves.Remove(gameSave);
                await _context.SaveChangesAsync(cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting save: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Synchronous Methods

        public bool SaveGame(string saveName, Character playerCharacter, string currentLocationName, int playTime)
        {
            return SaveGameAsync(saveName, playerCharacter, currentLocationName, playTime).GetAwaiter().GetResult();
        }

        public bool LoadGame(string saveName, out Character playerCharacter, out string currentLocationName, out int playTime)
        {
            var result = LoadGameAsync(saveName).GetAwaiter().GetResult();

            playerCharacter = result.PlayerCharacter;
            currentLocationName = result.CurrentLocationName;
            playTime = result.PlayTime;

            return result.Success;
        }

        public List<(string SaveName, DateTime SaveDate)> GetAllSaves()
        {
            return GetAllSavesAsync().GetAwaiter().GetResult();
        }

        public bool DeleteSave(string saveName)
        {
            return DeleteSaveAsync(saveName).GetAwaiter().GetResult();
        }

        #endregion

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
                    // Note: We don't dispose the _context here as it's injected
                    // and managed by the DI container
                }

                _disposed = true;
            }
        }

        ~GameSaveRepository()
        {
            Dispose(false);
        }

        #endregion
    }
}