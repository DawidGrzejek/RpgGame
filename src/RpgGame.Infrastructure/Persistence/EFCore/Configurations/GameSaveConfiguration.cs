using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RpgGame.Infrastructure.Persistence.EFCore.Configurations
{
    public class GameSaveConfiguration : IEntityTypeConfiguration<GameSave>
    {
        public void Configure(EntityTypeBuilder<GameSave> builder)
        {
            builder.HasKey(g => g.Id);

            builder.Property(g => g.SaveName)
                .IsRequired();

            builder.Property(g => g.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()"); // Use SQL Server default for created timestamp

            builder.Property(g => g.UpdatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()"); // Use SQL Server default for last modified timestamp
                
            // Configure JSON serialization for complex objects
            builder.Property(g => g.PlayerCharacterJson)
                .HasColumnName("player_character_json")
                .HasColumnType("nvarchar(max)"); // Use nvarchar(max) for JSON in SQL Server

            builder.HasIndex(g => g.SaveName)
                .IsUnique();

        }
    }
}