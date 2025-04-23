using Microsoft.EntityFrameworkCore;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Interfaces.World;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RpgGame.Infrastructure.Persistence.EFCore
{
    public class EfGameSaveRepository : IDisposable
    {
        private readonly GameDbContext _context;
        private bool _disposed = false;

        public EfGameSaveRepository()
        {
            _context = new GameDbContext();
            // Ensure database is created
            _context.Database.EnsureCreated();
        }

        public bool SaveGame(string saveName, Character playerCharacter, ILocation currentLocation, int playTime)
        {
            try
            {
                var existingSave = _context.GameSaves.FirstOrDefault(g => g.SaveName == saveName);

                if (existingSave != null)
                {
                    // Update existing save
                    existingSave.PlayerCharacter = playerCharacter;
                    existingSave.CurrentLocationName = currentLocation.Name;
                    existingSave.PlayTime = playTime;
                    existingSave.SaveDate = DateTime.Now;
                }
                else
                {
                    // Create new save
                    _context.GameSaves.Add(new GameSave
                    {
                        SaveName = saveName,
                        PlayerCharacter = playerCharacter,
                        CurrentLocationName = currentLocation.Name,
                        PlayTime = playTime,
                        SaveDate = DateTime.Now
                    });
                }

                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving game: {ex.Message}");
                return false;
            }
        }

        public bool LoadGame(string saveName, IGameWorld gameWorld, out Character playerCharacter, out ILocation currentLocation, out int playTime)
        {
            playerCharacter = null;
            currentLocation = null;
            playTime = 0;

            try
            {
                var gameSave = _context.GameSaves.FirstOrDefault(g => g.SaveName == saveName);

                if (gameSave == null)
                {
                    Console.WriteLine($"No save found with name: {saveName}");
                    return false;
                }

                // Get the player character from JSON
                playerCharacter = gameSave.PlayerCharacter;

                // Get the location
                currentLocation = gameWorld.GetLocation(gameSave.CurrentLocationName);
                if (currentLocation == null)
                {
                    // If location not found, use starting location
                    Console.WriteLine($"Location '{gameSave.CurrentLocationName}' not found. Using starting location.");
                    currentLocation = gameWorld.StartLocation;
                }

                // Get play time
                playTime = gameSave.PlayTime;

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading game: {ex.Message}");
                return false;
            }
        }

        public List<(string SaveName, DateTime SaveDate)> GetAllSaves()
        {
            try
            {
                return _context.GameSaves
                    .OrderByDescending(g => g.SaveDate)
                    .Select(g => new KeyValuePair<string, DateTime>(g.SaveName, g.SaveDate)) // Replace tuple literal with KeyValuePair
                    .ToList()
                    .Select(kvp => (kvp.Key, kvp.Value)) // Convert KeyValuePair back to tuple after materializing the query
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting saves: {ex.Message}");
                return new List<(string, DateTime)>();
            }
        }

        public bool DeleteSave(string saveName)
        {
            try
            {
                var gameSave = _context.GameSaves.FirstOrDefault(g => g.SaveName == saveName);

                if (gameSave == null)
                    return false;

                _context.GameSaves.Remove(gameSave);
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting save: {ex.Message}");
                return false;
            }
        }

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
                    _context?.Dispose();
                }

                _disposed = true;
            }
        }

        ~EfGameSaveRepository()
        {
            Dispose(false);
        }
    }
}