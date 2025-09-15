using System;

namespace RpgGame.Domain.Entities.EventSourcing
{
    /// <summary>
    /// Value object containing statistics about event storage performance and space usage
    /// Used for monitoring and optimization of the event sourcing system
    /// </summary>
    public record EventStorageStatistics(
        /// <summary>
        /// Total number of events in the system
        /// </summary>
        long TotalEventCount,
        
        /// <summary>
        /// Total number of compressed events
        /// </summary>
        long CompressedEventCount,
        
        /// <summary>
        /// Total number of event rollups
        /// </summary>
        long RollupCount,
        
        /// <summary>
        /// Total storage space used in bytes
        /// </summary>
        long TotalStorageBytes,
        
        /// <summary>
        /// Space saved through compression in bytes
        /// </summary>
        long CompressionSavingsBytes,
        
        /// <summary>
        /// Space saved through rollups in bytes
        /// </summary>
        long RollupSavingsBytes,
        
        /// <summary>
        /// Average event size in bytes
        /// </summary>
        double AverageEventSize,
        
        /// <summary>
        /// Oldest event timestamp in the system
        /// </summary>
        DateTime OldestEventTimestamp,
        
        /// <summary>
        /// Most recent event timestamp in the system
        /// </summary>
        DateTime NewestEventTimestamp,
        
        /// <summary>
        /// When these statistics were calculated
        /// </summary>
        DateTime CalculatedAt
    )
    {
        /// <summary>
        /// Gets the total space saved through optimization
        /// </summary>
        public long TotalSpaceSaved => CompressionSavingsBytes + RollupSavingsBytes;
        
        /// <summary>
        /// Gets the percentage of space saved
        /// </summary>
        public decimal SpaceSavingPercentage
        {
            get
            {
                var originalSize = TotalStorageBytes + TotalSpaceSaved;
                if (originalSize == 0) return 0m;
                return (decimal)TotalSpaceSaved / originalSize * 100m;
            }
        }
        
        /// <summary>
        /// Gets the compression ratio as a percentage
        /// </summary>
        public decimal CompressionRatio
        {
            get
            {
                if (CompressedEventCount == 0) return 0m;
                return (decimal)CompressionSavingsBytes / (CompressionSavingsBytes + (CompressedEventCount * (long)AverageEventSize)) * 100m;
            }
        }
        
        /// <summary>
        /// Gets the time span covered by events in the system
        /// </summary>
        public TimeSpan EventTimeSpan => NewestEventTimestamp - OldestEventTimestamp;
        
        /// <summary>
        /// Gets events per day rate
        /// </summary>
        public double EventsPerDay
        {
            get
            {
                var days = EventTimeSpan.TotalDays;
                if (days <= 0) return 0;
                return TotalEventCount / days;
            }
        }
        
        /// <summary>
        /// Creates statistics with zero values
        /// </summary>
        public static EventStorageStatistics Empty()
        {
            var now = DateTime.UtcNow;
            return new EventStorageStatistics(
                TotalEventCount: 0,
                CompressedEventCount: 0,
                RollupCount: 0,
                TotalStorageBytes: 0,
                CompressionSavingsBytes: 0,
                RollupSavingsBytes: 0,
                AverageEventSize: 0,
                OldestEventTimestamp: now,
                NewestEventTimestamp: now,
                CalculatedAt: now
            );
        }
    }
}