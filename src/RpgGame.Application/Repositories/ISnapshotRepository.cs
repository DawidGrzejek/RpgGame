using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RpgGame.Domain.Entities.EventSourcing;

namespace RpgGame.Application.Services
{
    /// <summary>
    /// Repository interface for character snapshot persistence
    /// </summary>
    public interface ISnapshotRepository
    {
        /// <summary>
        /// Gets the latest snapshot for a character
        /// </summary>
        Task<CharacterSnapshot> GetLatestSnapshotAsync(Guid characterId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets all snapshots for a character, ordered by creation date descending
        /// </summary>
        Task<IEnumerable<CharacterSnapshot>> GetSnapshotsAsync(Guid characterId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets a specific snapshot by ID
        /// </summary>
        Task<CharacterSnapshot> GetSnapshotAsync(Guid snapshotId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Saves a new snapshot
        /// </summary>
        Task SaveSnapshotAsync(CharacterSnapshot snapshot, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Marks all previous snapshots for a character as no longer being the latest
        /// </summary>
        Task MarkPreviousSnapshotsAsOldAsync(Guid characterId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets character IDs that need snapshots based on event count and age thresholds
        /// </summary>
        Task<IEnumerable<Guid>> GetCharactersNeedingSnapshotsAsync(
            int eventCountThreshold, 
            TimeSpan maxAge, 
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Deletes old snapshots, keeping only the specified number of recent ones per character
        /// </summary>
        Task<int> DeleteOldSnapshotsAsync(int keepCount, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets snapshot statistics for monitoring
        /// </summary>
        Task<SnapshotRepositoryStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets snapshots that are larger than the specified size threshold (for optimization)
        /// </summary>
        Task<IEnumerable<CharacterSnapshot>> GetLargeSnapshotsAsync(
            int sizeThreshold, 
            int limit = 100, 
            CancellationToken cancellationToken = default);
    }
    
    /// <summary>
    /// Statistics about the snapshot repository for monitoring and optimization
    /// </summary>
    public class SnapshotRepositoryStatistics
    {
        public int TotalSnapshots { get; set; }
        public int UniqueCharacters { get; set; }
        public long TotalStorageBytes { get; set; }
        public int AverageSnapshotSize { get; set; }
        public DateTime? OldestSnapshot { get; set; }
        public DateTime? NewestSnapshot { get; set; }
        public int SnapshotsCreatedToday { get; set; }
        public int SnapshotsCreatedThisWeek { get; set; }
        public Dictionary<string, int> SnapshotsByCharacterType { get; set; } = new();
    }
}