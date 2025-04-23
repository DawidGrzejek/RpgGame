using RpgGame.Application.Services;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Entities.World;
using RpgGame.Domain.Interfaces.World;
using RpgGame.Presentation.Views;
using System;

namespace RpgGame.Presentation
{
    public class Program
    {
        static void Main(string[] args)
        {
            bool exitGame = false;
            GameWorld gameWorld = new GameWorld();

            // Use using statement to ensure proper disposal of resources
            using (GameSaveService gameSaveService = new GameSaveService())
            {
                try
                {
                    while (!exitGame)
                    {
                        // Show main menu
                        var mainMenu = new MainMenuView();
                        int selection = mainMenu.Show();

                        switch (selection)
                        {
                            case 0: // New Game
                                StartNewGame(gameWorld, gameSaveService);
                                break;
                            case 1: // Load Game
                                LoadGame(gameWorld, gameSaveService);
                                break;
                            case 2: // Delete Save
                                DeleteSave(gameSaveService);
                                break;
                            case 3: // Options
                                ShowOptions();
                                break;
                            case 4: // Exit
                                exitGame = true;
                                Console.WriteLine("Thanks for playing!");
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Clear();
                    Console.WriteLine("===================================");
                    Console.WriteLine("=           ERROR                 =");
                    Console.WriteLine("===================================");
                    Console.WriteLine();
                    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    Console.WriteLine();
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                }
            }
        }

        // This method creates a new character and starts the game
        static void StartNewGame(GameWorld gameWorld, GameSaveService gameSaveService)
        {
            // Create character
            var characterCreationView = new CharacterCreationView();
            Character playerCharacter = characterCreationView.Show();

            if (playerCharacter == null)
            {
                Console.WriteLine("Character creation failed.");
                Console.ReadKey();
                return;
            }

            // Start a new play time session
            gameSaveService.StartNewSession();

            // Start game with the character and world - using the using statement for proper disposal
            using (var gameWorldView = new GameWorldView(playerCharacter, gameWorld, gameSaveService))
            {
                gameWorldView.Show();
            }
        }

        // This method loads a saved game
        static void LoadGame(GameWorld gameWorld, GameSaveService gameSaveService)
        {
            var saveLoadView = new SaveLoadGameView(gameSaveService);
            bool loadSuccessful = saveLoadView.ShowLoadGameInterface(gameWorld, out Character playerCharacter, out ILocation currentLocation);

            if (loadSuccessful && playerCharacter != null && currentLocation != null)
            {
                // Start game with the loaded character and location - using the using statement for proper disposal
                using (var gameWorldView = new GameWorldView(playerCharacter, gameWorld, gameSaveService))
                {
                    gameWorldView.Show();
                }
            }
        }

        // This method deletes a saved game
        static void DeleteSave(GameSaveService gameSaveService)
        {
            var saveLoadView = new SaveLoadGameView(gameSaveService);
            saveLoadView.ShowDeleteSaveInterface();
        }

        // This method shows the options menu
        static void ShowOptions()
        {
            Console.Clear();
            Console.WriteLine("===================================");
            Console.WriteLine("=            OPTIONS              =");
            Console.WriteLine("===================================");
            Console.WriteLine();
            Console.WriteLine("1. Game Settings");
            Console.WriteLine("2. Controls");
            Console.WriteLine("3. About");
            Console.WriteLine("4. Back to Main Menu");

            int option = GetPlayerInputOption(1, 4);

            switch (option)
            {
                case 1: // Game Settings
                    ShowGameSettings();
                    break;
                case 2: // Controls
                    ShowControls();
                    break;
                case 3: // About
                    ShowAbout();
                    break;
                case 4: // Back
                    return;
            }
        }

        // Helper method to get player input within a range
        static int GetPlayerInputOption(int min, int max)
        {
            int option = -1;
            do
            {
                Console.Write($"\nSelect an option ({min}-{max}): ");
                if (int.TryParse(Console.ReadLine(), out int input) && input >= min && input <= max)
                {
                    option = input;
                }
                else
                {
                    Console.WriteLine("Invalid option. Please try again.");
                }
            } while (option == -1);

            return option;
        }

        // Additional option menu methods
        static void ShowGameSettings()
        {
            Console.Clear();
            Console.WriteLine("===================================");
            Console.WriteLine("=          GAME SETTINGS          =");
            Console.WriteLine("===================================");
            Console.WriteLine();
            Console.WriteLine("Game settings would be configured here.");
            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
        }

        static void ShowControls()
        {
            Console.Clear();
            Console.WriteLine("===================================");
            Console.WriteLine("=            CONTROLS             =");
            Console.WriteLine("===================================");
            Console.WriteLine();
            Console.WriteLine("Number keys - Select menu options");
            Console.WriteLine("Enter - Confirm selection");
            Console.WriteLine("Any key - Continue through messages");
            Console.WriteLine();
            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
        }

        static void ShowAbout()
        {
            Console.Clear();
            Console.WriteLine("===================================");
            Console.WriteLine("=             ABOUT               =");
            Console.WriteLine("===================================");
            Console.WriteLine();
            Console.WriteLine("Simple Console RPG");
            Console.WriteLine("Version 1.0");
            Console.WriteLine("A text-based role-playing game.");
            Console.WriteLine();
            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
        }
    }
}