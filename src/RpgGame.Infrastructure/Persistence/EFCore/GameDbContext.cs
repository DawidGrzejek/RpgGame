using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Infrastructure.Persistence.EFCore
{
    public class GameDbContext : DbContext
    {
        public DbSet<GameSave> GameSaves { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Find the solution root directory by navigating up from the current assembly's location
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            string solutionDir = FindSolutionRootDirectory(currentDir);

            // Create a Data directory in the solution root if it doesn't exist
            string dataDir = Path.Combine(solutionDir, "Data");
            Directory.CreateDirectory(dataDir);

            string dbPath = Path.Combine(dataDir, "rpggame.db");

            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        private string FindSolutionRootDirectory(string startingPath)
        {
            // Start with the bin directory path
            DirectoryInfo directory = new DirectoryInfo(startingPath);

            // Navigate up until we find the solution directory (where .sln might be)
            // Or until we reach some reasonable upper bound (like 5 levels)
            int maxLevels = 5;
            int level = 0;

            while (directory != null && level < maxLevels)
            {
                // Check if this looks like a solution root (has src folder or .sln file)
                if (Directory.Exists(Path.Combine(directory.FullName, "src")) ||
                    directory.GetFiles("*.sln").Length > 0)
                {
                    return directory.FullName;
                }

                // Move up to the parent directory
                directory = directory.Parent;
                level++;
            }

            // If we couldn't find a solution root, just return a path relative to where the app is running
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure GameSave entity
            modelBuilder.Entity<GameSave>()
                .ToTable("GameSaves") // Explicit table name
                .HasKey(g => g.Id);

            modelBuilder.Entity<GameSave>()
                .Property(g => g.SaveName)
                .IsRequired();

            modelBuilder.Entity<GameSave>()
                .HasIndex(g => g.SaveName)
                .IsUnique();

            // Configure JSON serialization for complex objects
            modelBuilder.Entity<GameSave>()
                .Property(g => g.PlayerCharacterJson)
                .HasColumnName("PlayerCharacter");
        }
    }
}
