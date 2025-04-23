using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Entities.World;
using RpgGame.Presentation.Views;

bool exitGame = false;

while (!exitGame)
{
    // Show main menu
    var mainMenu = new MainMenuView();
    int selection = mainMenu.Show();

    switch (selection)
    {
        case 0: // New Game
            StartNewGame();
            break;
        case 1: // Load Game
                // Not implemented in this example
            Console.WriteLine("Load game not implemented yet.");
            Console.ReadKey();
            break;
        case 2: // Options
                // Not implemented in this example
            Console.WriteLine("Options not implemented yet.");
            Console.ReadKey();
            break;
        case 3: // Exit
            exitGame = true;
            Console.WriteLine("Thanks for playing!");
            break;
    }
}

// This method creates a new character and starts the game
static void StartNewGame()
{
    // Create character
    var characterCreationView = new CharacterCreationView();
    Character playerCharacter = characterCreationView.Show();

    // Create game world
    var gameWorld = new GameWorld();

    // Start game with the character and world
    var gameWorldView = new GameWorldView(playerCharacter, gameWorld);
    gameWorldView.Show();
}