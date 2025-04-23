using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Entities.World;
using RpgGame.Domain.Interfaces.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Presentation.Views
{
    /// <summary>
    /// The main exploration interface showing the current location description, available actions, and player status.
    /// </summary>
    public class GameWorldView
    {
        private readonly Character _player;
        private readonly GameWorld _gameWorld;
        private Location _currentLocation;

        public GameWorldView(Character player, GameWorld gameWorld)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _gameWorld = gameWorld ?? throw new ArgumentNullException(nameof(gameWorld));
            _currentLocation = gameWorld.StartLocation;
        }

        /// <summary>
        /// Displays the game world and runs the main game loop
        /// </summary>
        public void Show()
        {
            bool gameRunning = true;

            while (gameRunning && _player.Health > 0)
            {
                // Display current location and player status
                Console.Clear();
                Console.WriteLine($"Location: {_currentLocation.Name}");
                Console.WriteLine(new string('-', _currentLocation.Name.Length + 10));
                Console.WriteLine(_currentLocation.Description);
                Console.WriteLine();

                Console.WriteLine($"Player: {_player.Name} (Level {_player.Level})");
                Console.WriteLine($"Health: {_player.Health}/{_player.MaxHealth}");
                Console.WriteLine();

                // Display available actions
                Console.WriteLine("What would you like to do?");
                Console.WriteLine("1. Explore area");
                Console.WriteLine("2. Rest");
                Console.WriteLine("3. Travel to another location");
                Console.WriteLine("4. View character stats");
                Console.WriteLine("5. Return to main menu");

                // Get player action
                int action = GetPlayerAction(1, 5);

                // Process action
                switch (action)
                {
                    case 1: // Explore
                        ExploreArea();
                        break;
                    case 2: // Rest
                        Rest();
                        break;
                    case 3: // Travel
                        TravelToNewLocation();
                        break;
                    case 4: // View stats
                        ViewCharacterStats();
                        break;
                    case 5: // Return to main menu
                        gameRunning = false;
                        break;
                }

                // Check if player is still alive
                if (_player.Health <= 0)
                {
                    GameOver();
                }
                else if (gameRunning)
                {
                    // Pause before next action
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        // Helper method to get validated player input
        private int GetPlayerAction(int min, int max)
        {
            int action = -1;
            do
            {
                Console.WriteLine();
                Console.Write($"Select an option ({min}-{max}): ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out int option) && option >= min && option <= max)
                {
                    action = option;
                }
                else
                {
                    Console.WriteLine("Invalid option. Please try again.");
                }

            } while (action == -1);

            return action;
        }

        private void SetCurrentLocation(Location newLocation)
        {
            if (newLocation == null)
                throw new ArgumentNullException(nameof(newLocation));

            _currentLocation = newLocation;
            Console.WriteLine($"\nYou travel to {_currentLocation.Name}...");
        }

        // These methods would implement the different actions
        private void ExploreArea()
        {
            Console.WriteLine($"\nYou explore the {_currentLocation.Name}...");

            // Random encounter logic would go here
            // For now, just a placeholder
            Random rnd = new Random();
            int result = rnd.Next(3);

            switch (result)
            {
                case 0:
                    Console.WriteLine("You find nothing of interest.");
                    break;
                case 1:
                    Console.WriteLine("You find a small health potion and use it.");
                    _player.Heal(10);
                    break;
                case 2:
                    // This would call a combat view in a real implementation
                    Console.WriteLine("You encounter an enemy! (Combat system not implemented in this example)");
                    break;
            }
        }

        private void Rest()
        {
            Console.WriteLine("\nYou take some time to rest...");
            int healAmount = _player.MaxHealth / 5; // Heal 20% of max health
            _player.Heal(healAmount);
        }

        private void TravelToNewLocation()
        {
            Console.WriteLine("\nWhere would you like to travel?");

            // Get connected locations
            List<Location> connectedLocations = _gameWorld.GetConnectedLocations(_currentLocation);

            if (connectedLocations.Count == 0)
            {
                Console.WriteLine("There are no locations connected to your current location.");
                return;
            }

            // Display available locations
            for (int i = 0; i < connectedLocations.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {connectedLocations[i].Name}");
            }

            // Get player selection
            int selection = GetPlayerAction(1, connectedLocations.Count);

            // Set new location
            SetCurrentLocation(connectedLocations[selection - 1]);
            Console.WriteLine($"\nYou travel to {_currentLocation.Name}...");
        }

        private void ViewCharacterStats()
        {
            Console.Clear();
            Console.WriteLine("===================================");
            Console.WriteLine("=        CHARACTER STATS          =");
            Console.WriteLine("===================================");
            Console.WriteLine();
            Console.WriteLine($"Name: {_player.Name}");
            Console.WriteLine($"Class: {_player.GetType().Name}");
            Console.WriteLine($"Level: {_player.Level}");
            Console.WriteLine($"Health: {_player.Health}/{_player.MaxHealth}");
            Console.WriteLine($"Strength: {_player.Strength}");
            Console.WriteLine($"Defense: {_player.Defense}");

            // Additional stats would be displayed based on character class

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        private void GameOver()
        {
            Console.Clear();
            Console.WriteLine("===================================");
            Console.WriteLine("=           GAME OVER             =");
            Console.WriteLine("===================================");
            Console.WriteLine();
            Console.WriteLine($"{_player.Name} has been defeated!");
            Console.WriteLine();
            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey();
        }
    }
}
