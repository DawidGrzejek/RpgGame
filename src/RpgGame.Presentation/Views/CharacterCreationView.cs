using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Entities.Characters.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace RpgGame.Presentation.Views
{
    /// <summary>
    /// Shows character class options, handles name input, and displays initial character stats. Returns the created character to the game.
    /// </summary>
    public class CharacterCreationView
    {
        // Character class options with descriptions
        private readonly string[] _classOptions =
        {
            "Warrior - High health and strength, specializes in melee combat",
            "Mage - Lower health but powerful magical abilities",
            "Rogue - Balanced stats with critical strike abilities"
        };

        /// <summary>
        /// Displays the character creation interface and creates a new character
        /// </summary>
        /// <returns>The newly created character</returns>
        public Character Show()
        {
            Console.Clear();
            Console.WriteLine("===================================");
            Console.WriteLine("=        CHARACTER CREATION       =");
            Console.WriteLine("===================================");
            Console.WriteLine();

            // Step 1: Choose character class
            Console.WriteLine("Choose your character class:");
            for (int i = 0; i < _classOptions.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {_classOptions[i]}");
            }

            int classSelection = -1;
            do
            {
                Console.WriteLine();
                Console.Write("Select a class (1-3): ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out int option) && option >= 1 && option <= _classOptions.Length)
                {
                    classSelection = option - 1; // Convert to 0-based index
                }
                else
                {
                    Console.WriteLine("Invalid option. Please try again.");
                }

            } while (classSelection == -1);

            // Step 2: Enter character name
            string name = "";
            do
            {
                Console.WriteLine();
                Console.Write("Enter your character's name: ");
                name = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Name cannot be empty. Please try again.");
                }

            } while (string.IsNullOrWhiteSpace(name));

            // Create the character based on selection
            Character newCharacter = null;
            switch (classSelection)
            {
                case 0: // Warrior
                    newCharacter = Warrior.Create(name);
                    break;
                case 1: // Mage
                    //newCharacter = new Mage();
                    break;
                case 2: // Rogue
                    //newCharacter = new Rouge();
                    break;
            }

            // Display the created character's stats
            Console.Clear();
            Console.WriteLine("===================================");
            Console.WriteLine("=      CHARACTER CREATED!         =");
            Console.WriteLine("===================================");
            Console.WriteLine();
            Console.WriteLine($"Name: {newCharacter.Name}");
            Console.WriteLine($"Class: {newCharacter.GetType().Name}");
            Console.WriteLine($"Health: {newCharacter.Health}/{newCharacter.MaxHealth}");
            Console.WriteLine($"Strength: {newCharacter.Strength}");
            Console.WriteLine($"Defense: {newCharacter.Defense}");
            Console.WriteLine();
            Console.WriteLine("Press any key to begin your adventure...");
            Console.ReadKey();

            return newCharacter;
        }
    }
}
