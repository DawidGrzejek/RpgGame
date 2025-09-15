using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RpgGame.Domain.Entities.Configuration;
using System.Text.Json;

namespace RpgGame.Infrastructure.Persistence.EFCore.Configurations
{
    public class AbilityTemplateConfiguration : IEntityTypeConfiguration<AbilityTemplate>
    {
        public void Configure(EntityTypeBuilder<AbilityTemplate> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(a => a.AbilityType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(a => a.TargetType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(a => a.ManaCost)
                .IsRequired();

            builder.Property(a => a.Cooldown)
                .IsRequired();

            builder.Property(a => a.Range)
                .IsRequired();

            builder.Property(a => a.AnimationName)
                .HasMaxLength(200);

            builder.Property(a => a.SoundEffect)
                .HasMaxLength(200);

            builder.Property(a => a.Requirements)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions)null))
                .HasColumnType("nvarchar(max)");

            builder.OwnsMany(a => a.Effects, effect =>
            {
                effect.Property(e => e.EffectType)
                    .IsRequired()
                    .HasConversion<string>();

                effect.Property(e => e.BasePower)
                    .IsRequired();

                effect.Property(e => e.Duration)
                    .IsRequired();

                effect.Property(e => e.Parameters)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                        v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions)null))
                    .HasColumnType("nvarchar(max)");
            });

            builder.HasIndex(a => a.Name)
                .IsUnique();
        }
    }
}