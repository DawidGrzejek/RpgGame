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
            string dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            Directory.CreateDirectory(dataDir);
            string dbPath = Path.Combine(dataDir, "rpggame.db");

            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure GameSave entity
            modelBuilder.Entity<GameSave>()
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
