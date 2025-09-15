using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RpgGame.Domain.Entities.Configuration;
using System.Text.Json;

namespace RpgGame.Infrastructure.Persistence.EFCore.Configurations
{
    public class ItemTemplateConfiguration : IEntityTypeConfiguration<ItemTemplate>
    {
        public void Configure(EntityTypeBuilder<ItemTemplate> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(i => i.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(i => i.ItemType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(i => i.Value)
                .IsRequired();

            builder.Property(i => i.IsConsumable)
                .IsRequired();

            builder.Property(i => i.IsEquippable)
                .IsRequired();

            builder.Property(i => i.EquipmentSlot)
                .HasConversion<string>();

            builder.Property(i => i.StatModifiers)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, int>>(v, (JsonSerializerOptions)null))
                .HasColumnType("nvarchar(max)");

            builder.HasIndex(i => i.Name)
                .IsUnique();
        }
    }
}