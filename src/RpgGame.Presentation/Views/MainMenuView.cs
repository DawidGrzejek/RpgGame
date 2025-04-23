using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Presentation.Views
{
    /// <summary>
    /// Displays the game title, options like "New Game," "Load Game," "Options," and "Exit." Processes user selection to navigate to other views.
    /// </summary>
    public class MainMenuView
    {
        // Titles and options for the main menu
        private readonly string[] _menuOptions =
        {
            "New Game",
            "Load Game",
            "Options",
            "Exit Game"
        };

        /// <summary>
        /// Displays the main menu and processes user input
        /// </summary>
        /// <returns>The selected menu option (0-3)</returns>
        public int Show()
        {
            // Display the game title and menu
            Console.Clear();
            Console.WriteLine("===================================");
            Console.WriteLine("=       SIMPLE CONSOLE RPG        =");
            Console.WriteLine("===================================");
            Console.WriteLine();

            // Display menu options
            for (int i = 0; i < _menuOptions.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {_menuOptions[i]}");
            }

            // Get and validate user input
            int selectedOption = -1;
            do
            {
                Console.WriteLine();
                Console.Write("Select an option (1-4): ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out int option) && option >= 1 && option <= _menuOptions.Length)
                {
                    selectedOption = option - 1; // Return zero-based index
                }
                else
                {
                    Console.WriteLine("Invalid selection. Please try again.");
                }
            } while (selectedOption == -1);

            return selectedOption;
        }
    }
}
