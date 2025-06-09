using RpgGame.Application.Interfaces.Services;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Interfaces.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RpgGame.Presentation.Views
{
    /// <summary>
    /// Handles the save game and load game interfaces
    /// </summary>
    public class SaveLoadGameView
    {
        private readonly IGameSaveService _gameSaveService;

        public SaveLoadGameView(IGameSaveService gameSaveService)
        {
            _gameSaveService = gameSaveService ?? throw new ArgumentNullException(nameof(gameSaveService));
        }

        /// <summary>
        /// Shows the save game interface
        /// </summary>
        /// <param name="player">The player character to save</param>
        /// <param name="currentLocation">The current location to save</param>
        /// <returns>True if the game was saved successfully</returns>
        public async Task<bool> ShowSaveGameInterfaceAsync(Character player, ILocation currentLocation)
        {
            Console.Clear();
            Console.WriteLine("===================================");
            Console.WriteLine("=           SAVE GAME             =");
            Console.WriteLine("===================================");
            Console.WriteLine();

            // Show play time
            Console.WriteLine($"Current play time: {_gameSaveService.GetFormattedPlayTime()}");
            Console.WriteLine();

            // List existing saves
            List<(string SaveName, DateTime SaveDate)> existingSaves = await _gameSaveService.GetAvailableSavesAsync();
            if (existingSaves.Count > 0)
            {
                Console.WriteLine("Existing saves:");
                for (int i = 0; i < existingSaves.Count; i++)
                {
                    var (name, date) = existingSaves[i];
                    Console.WriteLine($"{i + 1}. {name} - {date}");
                }
                Console.WriteLine();
            }

            // Get save name from player
            Console.Write("Enter a name for your save (or press Enter to cancel): ");
            string saveName = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(saveName))
            {
                Console.WriteLine("Save cancelled.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return false;
            }

            // Check if save name already exists
            bool saveExists = existingSaves.Any(s => s.SaveName.Equals(saveName, StringComparison.OrdinalIgnoreCase));
            if (saveExists)
            {
                Console.Write($"A save with the name '{saveName}' already exists. Overwrite? (y/n): ");
                string overwrite = Console.ReadLine()?.Trim().ToLower();

                if (overwrite != "y" && overwrite != "yes")
                {
                    Console.WriteLine("Save cancelled.");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    return false;
                }
            }

            // Save the game
            bool result = await _gameSaveService.SaveGameAsync(saveName, player, currentLocation);

            if (result)
            {
                Console.WriteLine($"Game saved successfully as '{saveName}'.");
            }
            else
            {
                Console.WriteLine("Failed to save the game. Please try again.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return result;
        }

        /// <summary>
        /// Synchronous version of ShowSaveGameInterfaceAsync
        /// </summary>
        public bool ShowSaveGameInterface(Character player, ILocation currentLocation)
        {
            return ShowSaveGameInterfaceAsync(player, currentLocation).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Shows the load game interface
        /// </summary>
        /// <param name="gameWorld">The game world reference for location lookup</param>
        /// <returns>A tuple containing success flag, loaded player character, and current location</returns>
        public async Task<(bool Success, Character Player, ILocation CurrentLocation)> ShowLoadGameInterfaceAsync(IGameWorld gameWorld)
        {
            Console.Clear();
            Console.WriteLine("===================================");
            Console.WriteLine("=           LOAD GAME             =");
            Console.WriteLine("===================================");
            Console.WriteLine();

            // List existing saves
            List<(string SaveName, DateTime SaveDate)> existingSaves = await _gameSaveService.GetAvailableSavesAsync();
            if (existingSaves.Count == 0)
            {
                Console.WriteLine("No saved games found.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return (false, null, null);
            }

            Console.WriteLine("Available saves:");
            for (int i = 0; i < existingSaves.Count; i++)
            {
                var (name, date) = existingSaves[i];
                Console.WriteLine($"{i + 1}. {name} - {date}");
            }

            Console.WriteLine();
            Console.WriteLine("Enter the number of the save to load (or 0 to cancel): ");

            if (!int.TryParse(Console.ReadLine(), out int selection) || selection < 0 || selection > existingSaves.Count)
            {
                Console.WriteLine("Invalid selection.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return (false, null, null);
            }

            if (selection == 0)
            {
                Console.WriteLine("Load cancelled.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return (false, null, null);
            }

            // Get the selected save name
            string saveName = existingSaves[selection - 1].SaveName;

            // Load the game
            var result = await _gameSaveService.LoadGameAsync(saveName, gameWorld);

            if (result.Success)
            {
                Console.WriteLine($"Game '{saveName}' loaded successfully.");
            }
            else
            {
                Console.WriteLine($"Failed to load game '{saveName}'. Please try again.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return result;
        }

        /// <summary>
        /// Synchronous version of ShowLoadGameInterfaceAsync
        /// </summary>
        public bool ShowLoadGameInterface(IGameWorld gameWorld, out Character player, out ILocation currentLocation)
        {
            var result = ShowLoadGameInterfaceAsync(gameWorld).GetAwaiter().GetResult();
            player = result.Player;
            currentLocation = result.CurrentLocation;
            return result.Success;
        }

        /// <summary>
        /// Shows the delete save interface
        /// </summary>
        /// <returns>True if a save was deleted successfully</returns>
        public async Task<bool> ShowDeleteSaveInterfaceAsync()
        {
            Console.Clear();
            Console.WriteLine("===================================");
            Console.WriteLine("=          DELETE SAVE            =");
            Console.WriteLine("===================================");
            Console.WriteLine();

            // List existing saves
            List<(string SaveName, DateTime SaveDate)> existingSaves = await _gameSaveService.GetAvailableSavesAsync();
            if (existingSaves.Count == 0)
            {
                Console.WriteLine("No saved games found.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return false;
            }

            Console.WriteLine("Available saves:");
            for (int i = 0; i < existingSaves.Count; i++)
            {
                var (name, date) = existingSaves[i];
                Console.WriteLine($"{i + 1}. {name} - {date}");
            }

            Console.WriteLine();
            Console.WriteLine("Enter the number of the save to delete (or 0 to cancel): ");

            if (!int.TryParse(Console.ReadLine(), out int selection) || selection < 0 || selection > existingSaves.Count)
            {
                Console.WriteLine("Invalid selection.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return false;
            }

            if (selection == 0)
            {
                Console.WriteLine("Delete cancelled.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return false;
            }

            // Get the selected save name
            string saveName = existingSaves[selection - 1].SaveName;

            // Confirm deletion
            Console.Write($"Are you sure you want to delete '{saveName}'? (y/n): ");
            string confirm = Console.ReadLine()?.Trim().ToLower();

            if (confirm != "y" && confirm != "yes")
            {
                Console.WriteLine("Delete cancelled.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return false;
            }

            // Delete the save
            bool result = await _gameSaveService.DeleteSaveAsync(saveName);

            if (result)
            {
                Console.WriteLine($"Save '{saveName}' deleted successfully.");
            }
            else
            {
                Console.WriteLine($"Failed to delete save '{saveName}'. Please try again.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return result;
        }

        /// <summary>
        /// Synchronous version of ShowDeleteSaveInterfaceAsync
        /// </summary>
        public bool ShowDeleteSaveInterface()
        {
            return ShowDeleteSaveInterfaceAsync().GetAwaiter().GetResult();
        }
    }
}