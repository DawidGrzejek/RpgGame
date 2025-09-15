using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RpgGame.Application.Services;
using RpgGame.Domain.Entities.EventSourcing;
using RpgGame.Domain.Enums;
using RpgGame.Infrastructure.Persistence.EFCore;

namespace RpgGame.Infrastructure.Persistence.EFCore.Repositories
{
    /// <summary>
    /// Entity Framework implementation of the snapshot repository
    /// </summary>
    public class SnapshotRepository : ISnapshotRepository
    {
        private readonly GameDbContext _context;
        private readonly ILogger<SnapshotRepository> _logger;
        
        public SnapshotRepository(GameDbContext context, ILogger<SnapshotRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<CharacterSnapshot> GetLatestSnapshotAsync(Guid characterId, CancellationToken cancellationToken = default)
        {
            return await _context.CharacterSnapshots
                .Where(s => s.CharacterId == characterId && s.IsLatest)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }
        
        public async Task<IEnumerable<CharacterSnapshot>> GetSnapshotsAsync(Guid characterId, CancellationToken cancellationToken = default)
        {
            return await _context.CharacterSnapshots
                .Where(s => s.CharacterId == characterId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        
        public async Task<CharacterSnapshot> GetSnapshotAsync(Guid snapshotId, CancellationToken cancellationToken = default)
        {
            return await _context.CharacterSnapshots
                .FirstOrDefaultAsync(s => s.Id == snapshotId, cancellationToken);
        }
        
        public async Task SaveSnapshotAsync(CharacterSnapshot snapshot, CancellationToken cancellationToken = default)
        {
            _context.CharacterSnapshots.Add(snapshot);
            await _context.SaveChangesAsync(cancellationToken);
            
            _logger.LogDebug("Saved snapshot {SnapshotId} for character {CharacterId}", 
                snapshot.Id, snapshot.CharacterId);
        }
        
        public async Task MarkPreviousSnapshotsAsOldAsync(Guid characterId, CancellationToken cancellationToken = default)
        {
            var previousSnapshots = await _context.CharacterSnapshots
                .Where(s => s.CharacterId == characterId && s.IsLatest)
                .ToListAsync(cancellationToken);
            
            foreach (var snapshot in previousSnapshots)
            {
                snapshot.MarkAsOld();
            }
            
            if (previousSnapshots.Any())
            {
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogDebug("Marked {Count} previous snapshots as old for character {CharacterId}", 
                    previousSnapshots.Count, characterId);
            }
        }
        
        public async Task<IEnumerable<Guid>> GetCharactersNeedingSnapshotsAsync(
            int eventCountThreshold, 
            TimeSpan maxAge, 
            CancellationToken cancellationToken = default)
        {
            var cutoffDate = DateTime.UtcNow - maxAge;
            
            // This query would need to be optimized based on your event store structure
            // For now, this is a simplified version that would work with proper indexing
            var charactersWithOldSnapshots = await _context.CharacterSnapshots
                .Where(s => s.IsLatest && s.CreatedAt < cutoffDate)
                .Select(s => s.CharacterId)
                .ToListAsync(cancellationToken);
            
            // Characters with no snapshots that have enough events would need a more complex query
            // involving the Events table - this would typically be done with a stored procedure
            // or raw SQL for better performance
            
            return charactersWithOldSnapshots;
        }
        
        public async Task<int> DeleteOldSnapshotsAsync(int keepCount, CancellationToken cancellationToken = default)
        {
            var totalDeleted = 0;
            
            // Get all character IDs with snapshots
            var characterIds = await _context.CharacterSnapshots
                .Select(s => s.CharacterId)
                .Distinct()
                .ToListAsync(cancellationToken);
            
            foreach (var characterId in characterIds)
            {
                // Get snapshots for this character, keeping the most recent ones
                var snapshotsToDelete = await _context.CharacterSnapshots
                    .Where(s => s.CharacterId == characterId)
                    .OrderByDescending(s => s.CreatedAt)
                    .Skip(keepCount)
                    .ToListAsync(cancellationToken);
                
                if (snapshotsToDelete.Any())
                {
                    _context.CharacterSnapshots.RemoveRange(snapshotsToDelete);
                    totalDeleted += snapshotsToDelete.Count;
                }
            }
            
            if (totalDeleted > 0)
            {
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Deleted {Count} old snapshots", totalDeleted);
            }
            
            return totalDeleted;
        }
        
        public async Task<SnapshotRepositoryStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
        {
            var today = DateTime.UtcNow.Date;
            var weekAgo = DateTime.UtcNow.AddDays(-7);
            
            var stats = new SnapshotRepositoryStatistics();
            
            var snapshots = await _context.CharacterSnapshots
                .Select(s => new { s.CharacterType, s.StateSize, s.CreatedAt })
                .ToListAsync(cancellationToken);
            
            if (snapshots.Any())
            {
                stats.TotalSnapshots = snapshots.Count;
                stats.TotalStorageBytes = snapshots.Sum(s => (long)s.StateSize);
                stats.AverageSnapshotSize = (int)snapshots.Average(s => s.StateSize);
                stats.OldestSnapshot = snapshots.Min(s => s.CreatedAt);
                stats.NewestSnapshot = snapshots.Max(s => s.CreatedAt);
                stats.SnapshotsCreatedToday = snapshots.Count(s => s.CreatedAt.Date == today);
                stats.SnapshotsCreatedThisWeek = snapshots.Count(s => s.CreatedAt >= weekAgo);
                
                stats.SnapshotsByCharacterType = snapshots
                    .GroupBy(s => s.CharacterType.ToString())
                    .ToDictionary(g => g.Key, g => g.Count());
            }
            
            stats.UniqueCharacters = await _context.CharacterSnapshots
                .Select(s => s.CharacterId)
                .Distinct()
                .CountAsync(cancellationToken);
            
            return stats;
        }
        
        public async Task<IEnumerable<CharacterSnapshot>> GetLargeSnapshotsAsync(
            int sizeThreshold, 
            int limit = 100, 
            CancellationToken cancellationToken = default)
        {
            return await _context.CharacterSnapshots
                .Where(s => s.StateSize > sizeThreshold)
                .OrderByDescending(s => s.StateSize)
                .Take(limit)
                .ToListAsync(cancellationToken);
        }
    }
}