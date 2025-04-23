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

            // Start game with the character and world
            var gameWorldView = new GameWorldView(playerCharacter, gameWorld, gameSaveService);
            gameWorldView.Show();
        }

        // This method loads a saved game
        static void LoadGame(GameWorld gameWorld, GameSaveService gameSaveService)
        {
            var saveLoadView = new SaveLoadGameView(gameSaveService);
            bool loadSuccessful = saveLoadView.ShowLoadGameInterface(gameWorld, out Character playerCharacter, out ILocation currentLocation);

            if (loadSuccessful && playerCharacter != null && currentLocation != null)
            {
                // Start game with the loaded character and location
                var gameWorldView = new GameWorldView(playerCharacter, gameWorld, gameSaveService);
                gameWorldView.Show();
            }
        }

        // This method deletes a saved game
        static void DeleteSave(GameSaveService gameSaveService)
        {
            var saveLoadView = new SaveLoadGameView(gameSaveService);
            saveLoadView.ShowDeleteSaveInterface();
        }

        // This method shows the options menu (not implemented in this version)
        static void ShowOptions()
        {
            Console.Clear();
            Console.WriteLine("===================================");
            Console.WriteLine("=            OPTIONS              =");
            Console.WriteLine("===================================");
            Console.WriteLine();
            Console.WriteLine("Options menu not implemented yet.");
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}