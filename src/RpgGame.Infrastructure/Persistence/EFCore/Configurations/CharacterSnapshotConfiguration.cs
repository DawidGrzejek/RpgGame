using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RpgGame.Domain.Entities.EventSourcing;

namespace RpgGame.Infrastructure.Persistence.EFCore.Configurations
{
    /// <summary>
    /// Entity Framework configuration for CharacterSnapshot entity
    /// </summary>
    public class CharacterSnapshotConfiguration : IEntityTypeConfiguration<CharacterSnapshot>
    {
        public void Configure(EntityTypeBuilder<CharacterSnapshot> builder)
        {
            builder.ToTable("CharacterSnapshots");
            
            // Primary key
            builder.HasKey(s => s.Id);
            
            // Properties
            builder.Property(s => s.Id)
                .IsRequired()
                .ValueGeneratedNever();
                
            builder.Property(s => s.CharacterId)
                .IsRequired()
                .HasComment("The Character ID this snapshot represents");
                
            builder.Property(s => s.EventVersion)
                .IsRequired()
                .HasComment("The event version this snapshot captures up to");
                
            builder.Property(s => s.TotalEventCount)
                .IsRequired()
                .HasComment("Total number of events used to create this snapshot");
                
            builder.Property(s => s.CreatedAt)
                .IsRequired()
                .HasComment("When this snapshot was created");
                
            builder.Property(s => s.SerializedState)
                .IsRequired()
                .HasColumnType("NVARCHAR(MAX)")
                .HasComment("Serialized character state as JSON");
                
            builder.Property(s => s.CharacterName)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("Character name for quick lookups");
                
            builder.Property(s => s.CharacterLevel)
                .IsRequired()
                .HasComment("Character level at time of snapshot");
                
            builder.Property(s => s.CharacterType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasComment("Character type (Player/NPC)");
                
            builder.Property(s => s.IsLatest)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Indicates if this is the latest snapshot for the character");
                
            builder.Property(s => s.StateSize)
                .IsRequired()
                .HasComment("Size of serialized state in bytes");
                
            builder.Property(s => s.CreationDuration)
                .IsRequired()
                .HasComment("Time taken to create this snapshot");
            
            // Indexes for performance
            builder.HasIndex(s => new { s.CharacterId, s.IsLatest })
                .HasDatabaseName("IX_CharacterSnapshots_CharacterId_IsLatest")
                .HasFilter("[IsLatest] = 1");
                
            builder.HasIndex(s => s.CharacterId)
                .HasDatabaseName("IX_CharacterSnapshots_CharacterId");
                
            builder.HasIndex(s => s.CreatedAt)
                .HasDatabaseName("IX_CharacterSnapshots_CreatedAt");
                
            builder.HasIndex(s => new { s.CharacterType, s.CharacterLevel })
                .HasDatabaseName("IX_CharacterSnapshots_Type_Level");
                
            builder.HasIndex(s => s.StateSize)
                .HasDatabaseName("IX_CharacterSnapshots_StateSize");
        }
    }
}