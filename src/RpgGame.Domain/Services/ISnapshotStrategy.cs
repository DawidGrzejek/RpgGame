using System;
using System.Threading.Tasks;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Entities.EventSourcing;

namespace RpgGame.Domain.Services
{
    /// <summary>
    /// Strategy interface for determining when and how to create character snapshots
    /// </summary>
    public interface ISnapshotStrategy
    {
        /// <summary>
        /// Determines if a snapshot should be created based on the current character state and event count
        /// </summary>
        bool ShouldCreateSnapshot(Character character, int currentEventCount, CharacterSnapshot latestSnapshot);
        
        /// <summary>
        /// Gets the recommended maximum age for snapshots before they should be refreshed
        /// </summary>
        TimeSpan GetMaxSnapshotAge();
        
        /// <summary>
        /// Gets the event count threshold after which a snapshot should be considered
        /// </summary>
        int GetEventCountThreshold();
        
        /// <summary>
        /// Gets the minimum time interval between snapshots to prevent too frequent snapshot creation
        /// </summary>
        TimeSpan GetMinSnapshotInterval();
    }
    
    /// <summary>
    /// Default snapshot strategy with configurable thresholds
    /// </summary>
    public class DefaultSnapshotStrategy : ISnapshotStrategy
    {
        private readonly SnapshotConfiguration _config;
        
        public DefaultSnapshotStrategy(SnapshotConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }
        
        public bool ShouldCreateSnapshot(Character character, int currentEventCount, CharacterSnapshot latestSnapshot)
        {
            // No snapshot exists and we have enough events
            if (latestSnapshot == null)
                return currentEventCount >= _config.MinEventsForFirstSnapshot;
            
            // Check if snapshot is too old
            if (DateTime.UtcNow - latestSnapshot.CreatedAt > _config.MaxSnapshotAge)
                return true;
            
            // Check if too many new events since last snapshot
            var eventsSinceSnapshot = currentEventCount - latestSnapshot.TotalEventCount;
            if (eventsSinceSnapshot >= _config.EventCountThreshold)
                return true;
            
            // Check minimum interval between snapshots
            if (DateTime.UtcNow - latestSnapshot.CreatedAt < _config.MinSnapshotInterval)
                return false;
            
            // For high-level characters, snapshot more frequently
            if (character.Level >= _config.HighLevelThreshold)
            {
                return eventsSinceSnapshot >= _config.HighLevelEventThreshold;
            }
            
            return false;
        }
        
        public TimeSpan GetMaxSnapshotAge() => _config.MaxSnapshotAge;
        public int GetEventCountThreshold() => _config.EventCountThreshold;
        public TimeSpan GetMinSnapshotInterval() => _config.MinSnapshotInterval;
    }
    
    /// <summary>
    /// Configuration for snapshot creation strategy
    /// </summary>
    public class SnapshotConfiguration
    {
        /// <summary>
        /// Minimum events before creating the first snapshot
        /// </summary>
        public int MinEventsForFirstSnapshot { get; init; } = 100;
        
        /// <summary>
        /// Number of new events that trigger snapshot creation
        /// </summary>
        public int EventCountThreshold { get; init; } = 500;
        
        /// <summary>
        /// Maximum age of snapshot before it should be refreshed
        /// </summary>
        public TimeSpan MaxSnapshotAge { get; init; } = TimeSpan.FromDays(30);
        
        /// <summary>
        /// Minimum time between snapshots to prevent spam
        /// </summary>
        public TimeSpan MinSnapshotInterval { get; init; } = TimeSpan.FromHours(1);
        
        /// <summary>
        /// Character level considered "high level" for more frequent snapshots
        /// </summary>
        public int HighLevelThreshold { get; init; } = 50;
        
        /// <summary>
        /// Reduced event threshold for high-level characters
        /// </summary>
        public int HighLevelEventThreshold { get; init; } = 250;
        
        /// <summary>
        /// Maximum number of snapshots to keep per character for storage optimization
        /// </summary>
        public int MaxSnapshotsPerCharacter { get; init; } = 5;
        
        /// <summary>
        /// Batch size for background snapshot creation
        /// </summary>
        public int BackgroundProcessingBatchSize { get; init; } = 10;
    }
}