using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RpgGame.Domain.Entities.Users;

namespace RpgGame.Infrastructure.Persistence.EFCore.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Table configuration
            builder.HasKey(u => u.Id);

            // Basic properties
            builder.Property(u => u.AspNetUserId)
                .IsRequired()
                .HasMaxLength(450); // Standard ASP.NET Identity ID length

            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(u => u.LastLoginAt)
                .HasColumnType("datetime2");

            builder.Property(u => u.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Collection properties as JSON
            builder.Property(u => u.Roles)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                .HasMaxLength(500);

            builder.Property(u => u.CharacterIds)
                .HasConversion(
                    v => string.Join(',', v.Select(id => id.ToString())),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(Guid.Parse).ToList())
                .HasMaxLength(1000);

            // Configure UserPreferences as owned entity (embedded in same table)
            builder.OwnsOne(u => u.Preferences, prefs =>
            {
                prefs.Property(p => p.EmailNotifications)
                    .HasColumnName("email_notifications")
                    .HasDefaultValue(true);

                prefs.Property(p => p.GameSoundEnabled)
                    .HasColumnName("game_sound_enabled")
                    .HasDefaultValue(true);

                prefs.Property(p => p.Theme)
                    .HasColumnName("theme")
                    .HasMaxLength(20)
                    .HasDefaultValue("dark");

                prefs.Property(p => p.Language)
                    .HasColumnName("language")
                    .HasMaxLength(10)
                    .HasDefaultValue("en");
            });

            // Configure UserStatistics as owned entity (embedded in same table)
            builder.OwnsOne(u => u.Statistics, stats =>
            {
                stats.Property(s => s.TotalPlayTimeMinutes)
                    .HasDefaultValue(0);

                stats.Property(s => s.CharactersCreated)
                    .HasDefaultValue(0);

                stats.Property(s => s.TotalLogins)
                    .HasDefaultValue(0);

                stats.Property(s => s.HighestCharacterLevel)
                    .HasDefaultValue(0);

                stats.Property(s => s.TotalEnemiesDefeated)
                    .HasDefaultValue(0);

                stats.Property(s => s.TotalQuestsCompleted)
                    .HasDefaultValue(0);

                stats.Property(s => s.TotalDeaths)
                    .HasDefaultValue(0);

                stats.Property(s => s.FirstLoginDate)
                    .HasColumnType("datetime2");

                stats.Property(s => s.LastActiveDate)
                    .HasColumnType("datetime2");

                // Achievement flags
                stats.Property(s => s.HasCreatedFirstCharacter)
                    .HasDefaultValue(false);

                stats.Property(s => s.HasReachedLevel10)
                    .HasDefaultValue(false);

                stats.Property(s => s.HasReachedLevel50)
                    .HasDefaultValue(false);

                stats.Property(s => s.HasCompleted10Quests)
                    .HasDefaultValue(false);

                stats.Property(s => s.HasDefeated100Enemies)
                    .HasDefaultValue(false);
            });

            // Indexes for performance
            builder.HasIndex(u => u.AspNetUserId)
                .IsUnique();

            builder.HasIndex(u => u.Username)
                .IsUnique();

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.HasIndex(u => u.CreatedAt);

            builder.HasIndex(u => u.IsActive);
        }
    }
}