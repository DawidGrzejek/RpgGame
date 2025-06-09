using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RpgGame.Application.Services;
using RpgGame.Domain;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Entities.World;
using RpgGame.Domain.Interfaces.World;
using RpgGame.Infrastructure;
using RpgGame.Presentation.Views;
using System;

namespace RpgGame.Presentation
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            /*// Setup configuration  
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory) // Use AppContext.BaseDirectory for compatibility  
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // Setup dependency injection  
            var serviceProvider = ConfigureServices(configuration);

            // Get required services  
            var gameWorld = serviceProvider.GetRequiredService<IGameWorld>();
            var gameSaveService = serviceProvider.GetRequiredService<Application.Interfaces.Services.IGameSaveService>();

            bool exitGame = false;

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
                            await StartNewGame(serviceProvider, gameWorld, gameSaveService);
                            break;
                        case 1: // Load Game  
                            await LoadGame(serviceProvider, gameWorld, gameSaveService);
                            break;
                        case 2: // Delete Save  
                            await DeleteSave(serviceProvider, gameSaveService);
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
            finally
            {
                // Clean up  
                if (serviceProvider is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }*/
        }

        private static ServiceProvider ConfigureServices(IConfiguration configuration)
        {
            // Create service collection  
            var services = new ServiceCollection();

            // Add configuration  
            services.AddSingleton<IConfiguration>(configuration);

            // Register layers in the correct order (inner to outer)  

            // 1. Domain layer (doesn't depend on other layers)  
            services.AddDomainServices();

            // 2. Application layer (depends on Domain)  
            services.AddApplicationServices();

            // 3. Infrastructure layer (implements Application interfaces)  
            services.AddInfrastructureServices(configuration);

            // You could add presentation services here if needed  

            // Build the service provider  
            return services.BuildServiceProvider();
        }
    }
}