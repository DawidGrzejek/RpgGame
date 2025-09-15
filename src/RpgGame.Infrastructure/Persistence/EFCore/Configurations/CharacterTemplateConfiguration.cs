using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RpgGame.Domain.Entities.Configuration;
using RpgGame.Domain.ValueObjects;
using System.Text.Json;

namespace RpgGame.Infrastructure.Persistence.EFCore.Configurations
{
    public class CharacterTemplateConfiguration : IEntityTypeConfiguration<CharacterTemplate>
    {
        public void Configure(EntityTypeBuilder<CharacterTemplate> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(c => c.CharacterType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(c => c.NPCBehavior)
                .HasConversion<string>();

            builder.Property(c => c.PlayerClass)
                .HasConversion<string>();

            builder.OwnsOne(c => c.BaseStats, stats =>
            {
                stats.Property(s => s.Level).IsRequired();
                stats.Property(s => s.CurrentHealth).IsRequired();
                stats.Property(s => s.MaxHealth).IsRequired();
                stats.Property(s => s.Strength).IsRequired();
                stats.Property(s => s.Defense).IsRequired();
                stats.Property(s => s.Speed).IsRequired();
                stats.Property(s => s.Magic).IsRequired();
            });

            builder.Property(c => c.ConfigurationData)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions)null))
                .HasColumnType("nvarchar(max)");

            builder.Property(c => c.AbilityIds)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions)null))
                .HasColumnType("nvarchar(max)");

            builder.Property(c => c.LootTableIds)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions)null))
                .HasColumnType("nvarchar(max)");

            builder.Property(c => c.BehaviorData)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions)null))
                .HasColumnType("nvarchar(max)");

            builder.HasIndex(c => c.Name)
                .IsUnique();
        }
    }
}