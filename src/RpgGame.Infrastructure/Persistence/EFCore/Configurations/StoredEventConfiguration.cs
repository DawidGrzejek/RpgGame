using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RpgGame.Domain.Events.Base;

namespace RpgGame.Infrastructure.Persistence.EFCore.Configurations
{
    public class StoredEventConfiguration : IEntityTypeConfiguration<StoredEvent>
    {
        public void Configure(EntityTypeBuilder<StoredEvent> builder)
        {
            //builder.ToTable("stored_events"); // Explicit table name

            builder.HasKey(e => e.Id);

            builder.Property(e => e.AggregateId)
                .IsRequired();

            builder.Property(e => e.AggregateType)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.EventType)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.EventData)
                .IsRequired()
                .HasColumnType("nvarchar(max)"); // Use nvarchar(max) for JSON storage in SQL Server

            builder.Property(e => e.Timestamp)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()"); // Use SQL Server default for timestamp

            builder.Property(e => e.Version)
                .IsRequired();

            builder.Property(e => e.UserId)
                .HasMaxLength(255); // Optional: UserId can be null, so no IsRequired()


            // Create indexes for efficient querying
            builder.HasIndex(e => e.AggregateId);
            builder.HasIndex(e => e.Timestamp);
            builder.HasIndex(e => e.EventType);

            builder.HasIndex(e => new { e.AggregateId, e.Version }).IsUnique();
            builder.HasIndex(e => new { e.AggregateId, e.Timestamp });
        }
    }
}