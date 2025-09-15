using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RpgGame.Application.Events;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Entities.EventSourcing;
using RpgGame.Domain.Services;

namespace RpgGame.Application.Services
{
    /// <summary>
    /// Service responsible for creating, managing, and using character snapshots for performance optimization
    /// </summary>
    public interface ISnapshotService
    {
        /// <summary>
        /// Gets a character by ID using the optimal approach (snapshot + incremental events)
        /// </summary>
        Task<Character> GetCharacterByIdAsync(Guid characterId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Creates a snapshot for a character if the strategy determines it's needed
        /// </summary>
        Task<CharacterSnapshot> CreateSnapshotIfNeededAsync(Guid characterId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Forces creation of a snapshot for a character (for administrative purposes)
        /// </summary>
        Task<CharacterSnapshot> CreateSnapshotAsync(Guid characterId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets snapshot statistics for monitoring and optimization
        /// </summary>
        Task<SnapshotStatistics> GetSnapshotStatisticsAsync(Guid characterId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Background task to process snapshots for characters that need them
        /// </summary>
        Task ProcessPendingSnapshotsAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Cleans up old snapshots based on retention policy
        /// </summary>
        Task CleanupOldSnapshotsAsync(CancellationToken cancellationToken = default);
    }
    
    public class SnapshotService : ISnapshotService
    {
        private readonly ISnapshotRepository _snapshotRepository;
        private readonly IEventStoreRepository _eventStore;
        private readonly ISnapshotStrategy _strategy;
        private readonly ILogger<SnapshotService> _logger;
        private readonly JsonSerializerSettings _jsonSettings;
        
        public SnapshotService(
            ISnapshotRepository snapshotRepository,
            IEventStoreRepository eventStore,
            ISnapshotStrategy strategy,
            ILogger<SnapshotService> logger)
        {
            _snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            _jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };
        }
        
        public async Task<Character> GetCharacterByIdAsync(Guid characterId, CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                // Get the latest snapshot
                var latestSnapshot = await _snapshotRepository.GetLatestSnapshotAsync(characterId, cancellationToken);
                
                if (latestSnapshot == null)
                {
                    // No snapshot exists, reconstruct from all events (fallback to original method)
                    _logger.LogInformation("No snapshot found for character {CharacterId}, reconstructing from all events", characterId);
                    return await ReconstructFromAllEventsAsync(characterId, cancellationToken);
                }
                
                // Reconstruct from snapshot + incremental events
                var character = await ReconstructFromSnapshotAsync(latestSnapshot, cancellationToken);
                
                stopwatch.Stop();
                _logger.LogInformation("Character {CharacterId} reconstructed from snapshot in {ElapsedMs}ms (snapshot from version {Version})", 
                    characterId, stopwatch.ElapsedMilliseconds, latestSnapshot.EventVersion);
                
                // Check if we need to create a new snapshot
                _ = Task.Run(() => CreateSnapshotIfNeededAsync(characterId, cancellationToken), cancellationToken);
                
                return character;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get character {CharacterId} using snapshots, falling back to full event reconstruction", characterId);
                // Fallback to full event reconstruction
                return await ReconstructFromAllEventsAsync(characterId, cancellationToken);
            }
        }
        
        public async Task<CharacterSnapshot> CreateSnapshotIfNeededAsync(Guid characterId, CancellationToken cancellationToken = default)
        {
            try
            {
                // Get current event count
                var allEvents = await _eventStore.GetEventsAsync(characterId);
                var eventList = allEvents.ToList();
                var currentEventCount = eventList.Count;
                
                if (currentEventCount == 0)
                {
                    _logger.LogDebug("No events found for character {CharacterId}, skipping snapshot", characterId);
                    return null;
                }
                
                // Get latest snapshot
                var latestSnapshot = await _snapshotRepository.GetLatestSnapshotAsync(characterId, cancellationToken);
                
                // Reconstruct character to check current state
                var character = latestSnapshot == null 
                    ? Character.FromEvents(characterId, eventList)
                    : await ReconstructFromSnapshotAsync(latestSnapshot, cancellationToken);
                
                // Check if snapshot is needed
                if (!_strategy.ShouldCreateSnapshot(character, currentEventCount, latestSnapshot))
                {
                    _logger.LogDebug("Snapshot not needed for character {CharacterId} (events: {EventCount}, last snapshot: {LastSnapshot})", 
                        characterId, currentEventCount, latestSnapshot?.CreatedAt);
                    return null;
                }
                
                return await CreateSnapshotAsync(characterId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check snapshot needs for character {CharacterId}", characterId);
                return null;
            }
        }
        
        public async Task<CharacterSnapshot> CreateSnapshotAsync(Guid characterId, CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                // Get all events
                var allEvents = await _eventStore.GetEventsAsync(characterId);
                var eventList = allEvents.ToList();
                
                if (!eventList.Any())
                {
                    throw new InvalidOperationException($"Cannot create snapshot for character {characterId} - no events found");
                }
                
                // Reconstruct character
                var character = Character.FromEvents(characterId, eventList);
                
                // Serialize character state
                var serializedState = JsonConvert.SerializeObject(character, _jsonSettings);
                
                stopwatch.Stop();
                
                // Create snapshot
                var snapshot = CharacterSnapshot.Create(
                    characterId,
                    character.Name,
                    character.Level,
                    character.Type,
                    eventList.Max(e => e.Version),
                    eventList.Count,
                    serializedState,
                    stopwatch.Elapsed);
                
                // Mark previous snapshots as old
                await _snapshotRepository.MarkPreviousSnapshotsAsOldAsync(characterId, cancellationToken);
                
                // Save new snapshot
                await _snapshotRepository.SaveSnapshotAsync(snapshot, cancellationToken);
                
                _logger.LogInformation("Created snapshot for character {CharacterId} ({CharacterName}) at version {Version} with {EventCount} events in {ElapsedMs}ms", 
                    characterId, character.Name, snapshot.EventVersion, snapshot.TotalEventCount, stopwatch.ElapsedMilliseconds);
                
                return snapshot;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Failed to create snapshot for character {CharacterId} after {ElapsedMs}ms", 
                    characterId, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
        
        public async Task<SnapshotStatistics> GetSnapshotStatisticsAsync(Guid characterId, CancellationToken cancellationToken = default)
        {
            var snapshots = await _snapshotRepository.GetSnapshotsAsync(characterId, cancellationToken);
            var allEvents = await _eventStore.GetEventsAsync(characterId);
            var eventCount = allEvents.Count();
            
            var snapshotList = snapshots.ToList();
            var latestSnapshot = snapshotList.FirstOrDefault(s => s.IsLatest);
            
            return new SnapshotStatistics
            {
                CharacterId = characterId,
                TotalEventCount = eventCount,
                TotalSnapshots = snapshotList.Count,
                LatestSnapshotVersion = latestSnapshot?.EventVersion ?? 0,
                EventsSinceLastSnapshot = latestSnapshot != null ? eventCount - latestSnapshot.TotalEventCount : eventCount,
                LastSnapshotCreated = latestSnapshot?.CreatedAt,
                AverageSnapshotSize = snapshotList.Any() ? (int)snapshotList.Average(s => s.StateSize) : 0,
                RecommendSnapshotCreation = latestSnapshot == null || 
                    _strategy.ShouldCreateSnapshot(null, eventCount, latestSnapshot)
            };
        }
        
        public async Task ProcessPendingSnapshotsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // This would be called by a background service
                var charactersNeedingSnapshots = await _snapshotRepository.GetCharactersNeedingSnapshotsAsync(
                    _strategy.GetEventCountThreshold(), 
                    _strategy.GetMaxSnapshotAge(), 
                    cancellationToken);
                
                var batchSize = 10; // Process in batches to avoid overwhelming the system
                var processed = 0;
                
                foreach (var characterId in charactersNeedingSnapshots.Take(batchSize))
                {
                    if (cancellationToken.IsCancellationRequested) break;
                    
                    try
                    {
                        await CreateSnapshotIfNeededAsync(characterId, cancellationToken);
                        processed++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process snapshot for character {CharacterId}", characterId);
                    }
                }
                
                if (processed > 0)
                {
                    _logger.LogInformation("Processed {ProcessedCount} character snapshots", processed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process pending snapshots");
            }
        }
        
        public async Task CleanupOldSnapshotsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var deletedCount = await _snapshotRepository.DeleteOldSnapshotsAsync(5, cancellationToken); // Keep only 5 snapshots per character
                if (deletedCount > 0)
                {
                    _logger.LogInformation("Cleaned up {DeletedCount} old snapshots", deletedCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cleanup old snapshots");
            }
        }
        
        private async Task<Character> ReconstructFromAllEventsAsync(Guid characterId, CancellationToken cancellationToken)
        {
            var events = await _eventStore.GetEventsAsync(characterId);
            return Character.FromEvents(characterId, events);
        }
        
        private async Task<Character> ReconstructFromSnapshotAsync(CharacterSnapshot snapshot, CancellationToken cancellationToken)
        {
            // Deserialize character from snapshot
            var character = JsonConvert.DeserializeObject<Character>(snapshot.SerializedState, _jsonSettings);
            
            // Get events since snapshot
            var incrementalEvents = await _eventStore.GetEventsAsync(snapshot.CharacterId, snapshot.EventVersion + 1);
            
            // Apply incremental events
            foreach (var @event in incrementalEvents.OrderBy(e => e.Version))
            {
                // Apply the event to the character (this would use reflection similar to EventSourcingService)
                ApplyEventToCharacter(character, @event);
            }
            
            return character;
        }
        
        private void ApplyEventToCharacter(Character character, Domain.Events.Base.IDomainEvent @event)
        {
            // This mirrors the logic in EventSourcingService.ApplyEventToAggregate
            var eventType = @event.GetType();
            var applyMethod = character.GetType().GetMethod("Apply",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic,
                null, new[] { eventType }, null);
            
            applyMethod?.Invoke(character, new object[] { @event });
        }
    }
    
    /// <summary>
    /// Statistics about character snapshots for monitoring and optimization
    /// </summary>
    public class SnapshotStatistics
    {
        public Guid CharacterId { get; set; }
        public int TotalEventCount { get; set; }
        public int TotalSnapshots { get; set; }
        public int LatestSnapshotVersion { get; set; }
        public int EventsSinceLastSnapshot { get; set; }
        public DateTime? LastSnapshotCreated { get; set; }
        public int AverageSnapshotSize { get; set; }
        public bool RecommendSnapshotCreation { get; set; }
    }
}