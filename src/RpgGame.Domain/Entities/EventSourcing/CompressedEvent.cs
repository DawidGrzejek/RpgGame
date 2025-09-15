using System;
using RpgGame.Domain.Base;

namespace RpgGame.Domain.Entities.EventSourcing
{
    /// <summary>
    /// Represents a compressed event for archival storage to optimize disk space
    /// </summary>
    public class CompressedEvent : DomainEntity
    {
        /// <summary>
        /// The Character ID this compressed event belongs to
        /// </summary>
        public Guid CharacterId { get; private set; }
        
        /// <summary>
        /// Original event ID for reference
        /// </summary>
        public Guid OriginalEventId { get; private set; }
        
        /// <summary>
        /// Event type name for deserialization
        /// </summary>
        public string EventType { get; private set; }
        
        /// <summary>
        /// Compressed event data using gzip or similar
        /// </summary>
        public byte[] CompressedData { get; private set; }
        
        /// <summary>
        /// Original size in bytes before compression
        /// </summary>
        public int OriginalSize { get; private set; }
        
        /// <summary>
        /// Compressed size in bytes after compression
        /// </summary>
        public int CompressedSize { get; private set; }
        
        /// <summary>
        /// Original timestamp when event occurred
        /// </summary>
        public DateTime OriginalTimestamp { get; private set; }
        
        /// <summary>
        /// When this event was compressed and archived
        /// </summary>
        public DateTime ArchivedAt { get; private set; }

        // EF Core constructor
        private CompressedEvent() { }

        /// <summary>
        /// Creates a new compressed event
        /// </summary>
        public static CompressedEvent Create(
            Guid characterId,
            Guid originalEventId,
            string eventType,
            byte[] compressedData,
            int originalSize,
            DateTime originalTimestamp)
        {
            var compressedEvent = new CompressedEvent
            {
                Id = Guid.NewGuid(),
                CharacterId = characterId,
                OriginalEventId = originalEventId,
                EventType = eventType,
                CompressedData = compressedData,
                OriginalSize = originalSize,
                CompressedSize = compressedData.Length,
                OriginalTimestamp = originalTimestamp,
                ArchivedAt = DateTime.UtcNow
            };

            return compressedEvent;
        }
        
        /// <summary>
        /// Gets compression ratio as percentage
        /// </summary>
        public decimal GetCompressionRatio()
        {
            if (OriginalSize == 0) return 0m;
            return (decimal)(OriginalSize - CompressedSize) / OriginalSize * 100m;
        }
    }
}