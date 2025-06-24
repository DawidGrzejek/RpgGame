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
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP"); // Use database default for created timestamp

            builder.Property(g => g.UpdatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP"); // Use database default for last modified timestamp
                
            // Configure JSON serialization for complex objects
            builder.Property(g => g.PlayerCharacterJson)
                .HasColumnName("player_character_json")
                .HasColumnType("jsonb");

            builder.HasIndex(g => g.SaveName)
                .IsUnique();

        }
    }
}