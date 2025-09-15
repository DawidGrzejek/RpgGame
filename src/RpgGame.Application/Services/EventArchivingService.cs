using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RpgGame.Application.Events;
using RpgGame.Domain.Events.Base;
using RpgGame.Domain.Entities.EventSourcing;
using System.Text.Json;

namespace RpgGame.Application.Services
{
    /// <summary>
    /// Service responsible for archiving old events to optimize storage and performance
    /// while maintaining data integrity and audit trails
    /// </summary>
    public interface IEventArchivingService
    {
        /// <summary>
        /// Archives events older than the specified age for characters that have recent snapshots
        /// </summary>
        Task<EventArchivingResult> ArchiveOldEventsAsync(
            TimeSpan maxEventAge, 
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Compresses event sequences by removing redundant intermediate state changes
        /// </summary>
        Task<EventCompressionResult> CompressEventHistoryAsync(
            Guid characterId, 
            int keepRecentEventCount = 1000,
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Creates event rollup snapshots that represent consolidated state changes over time
        /// </summary>
        Task<EventRollupResult> CreateEventRollupsAsync(
            Guid characterId,
            TimeSpan rollupInterval,
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets statistics about event storage and archiving for monitoring
        /// </summary>
        Task<EventStorageStatistics> GetStorageStatisticsAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Validates that archived events can still reconstruct character state correctly
        /// </summary>
        Task<ArchiveValidationResult> ValidateArchivedDataAsync(
            Guid characterId, 
            CancellationToken cancellationToken = default);
    }
    
    public class EventArchivingService : IEventArchivingService
    {
        private readonly IEventStoreRepository _eventStore;
        private readonly ISnapshotService _snapshotService;
        private readonly IEventArchiveRepository _archiveRepository;
        private readonly IEventSerializationService _eventSerializer;
        private readonly ILogger<EventArchivingService> _logger;
        private readonly EventArchivingConfiguration _config;
        
        public EventArchivingService(
            IEventStoreRepository eventStore,
            ISnapshotService snapshotService,
            IEventArchiveRepository archiveRepository,
            IEventSerializationService eventSerializer,
            ILogger<EventArchivingService> logger,
            EventArchivingConfiguration config)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _snapshotService = snapshotService ?? throw new ArgumentNullException(nameof(snapshotService));
            _archiveRepository = archiveRepository ?? throw new ArgumentNullException(nameof(archiveRepository));
            _eventSerializer = eventSerializer ?? throw new ArgumentNullException(nameof(eventSerializer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }
        
        public async Task<EventArchivingResult> ArchiveOldEventsAsync(
            TimeSpan maxEventAge, 
            CancellationToken cancellationToken = default)
        {
            var result = new EventArchivingResult();
            var cutoffDate = DateTime.UtcNow - maxEventAge;
            
            try
            {
                // Get characters that have snapshots newer than the cutoff date
                var eligibleCharacters = await _archiveRepository.GetCharactersEligibleForArchivingAsync(
                    cutoffDate, cancellationToken);
                
                foreach (var characterId in eligibleCharacters.Take(_config.MaxCharactersPerBatch))
                {
                    if (cancellationToken.IsCancellationRequested) break;
                    
                    var characterResult = await ArchiveCharacterEventsAsync(characterId, cutoffDate, cancellationToken);
                    result.CharactersProcessed++;
                    result.EventsArchived += characterResult.EventsArchived;
                    result.StorageFreed += characterResult.StorageFreed;
                    
                    if (characterResult.HasErrors)
                    {
                        result.Errors.AddRange(characterResult.Errors);
                        result.CharactersWithErrors++;
                    }
                }
                
                result.Success = true;
                _logger.LogInformation("Archived {EventCount} events for {CharacterCount} characters, freed {StorageSize} bytes", 
                    result.EventsArchived, result.CharactersProcessed, result.StorageFreed);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add($"Archive operation failed: {ex.Message}");
                _logger.LogError(ex, "Failed to archive old events");
            }
            
            return result;
        }
        
        public async Task<EventCompressionResult> CompressEventHistoryAsync(
            Guid characterId, 
            int keepRecentEventCount = 1000,
            CancellationToken cancellationToken = default)
        {
            var result = new EventCompressionResult { CharacterId = characterId };
            
            try
            {
                // Get all events for the character
                var allEvents = await _eventStore.GetEventsAsync(characterId);
                var eventList = allEvents.OrderBy(e => e.Version).ToList();
                
                if (eventList.Count <= keepRecentEventCount)
                {
                    result.Message = "No compression needed - event count below threshold";
                    return result;
                }
                
                // Keep recent events and identify candidates for compression
                var recentEvents = eventList.TakeLast(keepRecentEventCount).ToList();
                var oldEvents = eventList.Except(recentEvents).ToList();
                
                // Create compressed event sequence (this is a simplified example)
                var compressedEvents = CompressEventSequence(oldEvents);
                
                // Save compressed events to archive and remove originals
                await _archiveRepository.SaveCompressedEventsAsync(characterId, compressedEvents, cancellationToken);
                
                result.Success = true;
                result.OriginalEventCount = oldEvents.Count;
                result.CompressedEventCount = compressedEvents.Count;
                result.CompressionRatio = (double)compressedEvents.Count / oldEvents.Count;
                result.StorageSaved = CalculateStorageSaved(oldEvents, compressedEvents);
                
                _logger.LogInformation("Compressed {OriginalCount} events to {CompressedCount} for character {CharacterId} (ratio: {Ratio:P1})", 
                    result.OriginalEventCount, result.CompressedEventCount, characterId, result.CompressionRatio);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Compression failed: {ex.Message}";
                _logger.LogError(ex, "Failed to compress event history for character {CharacterId}", characterId);
            }
            
            return result;
        }
        
        public async Task<EventRollupResult> CreateEventRollupsAsync(
            Guid characterId,
            TimeSpan rollupInterval,
            CancellationToken cancellationToken = default)
        {
            var result = new EventRollupResult { CharacterId = characterId };
            
            try
            {
                // Get events grouped by time intervals
                var events = await _eventStore.GetEventsAsync(characterId);
                var eventGroups = GroupEventsByInterval(events, rollupInterval);
                
                var rollupsCreated = 0;
                foreach (var group in eventGroups)
                {
                    if (cancellationToken.IsCancellationRequested) break;
                    
                    var rollup = CreateRollupFromEvents(characterId, group.Key, group.Value);
                    await _archiveRepository.SaveEventRollupAsync(rollup, cancellationToken);
                    rollupsCreated++;
                }
                
                result.Success = true;
                result.RollupsCreated = rollupsCreated;
                result.Message = $"Created {rollupsCreated} event rollups";
                
                _logger.LogInformation("Created {RollupCount} event rollups for character {CharacterId}", 
                    rollupsCreated, characterId);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Rollup creation failed: {ex.Message}";
                _logger.LogError(ex, "Failed to create event rollups for character {CharacterId}", characterId);
            }
            
            return result;
        }
        
        public async Task<EventStorageStatistics> GetStorageStatisticsAsync(CancellationToken cancellationToken = default)
        {
            return await _archiveRepository.GetStorageStatisticsAsync(cancellationToken);
        }
        
        public async Task<ArchiveValidationResult> ValidateArchivedDataAsync(
            Guid characterId, 
            CancellationToken cancellationToken = default)
        {
            var result = new ArchiveValidationResult { CharacterId = characterId };
            
            try
            {
                // Get character using current reconstruction method
                var currentCharacter = await _snapshotService.GetCharacterByIdAsync(characterId, cancellationToken);
                
                // Get character using archived data only
                var archivedCharacter = await ReconstructFromArchivedDataAsync(characterId, cancellationToken);
                
                // Compare states
                result.Success = CompareCharacterStates(currentCharacter, archivedCharacter);
                result.Message = result.Success 
                    ? "Archived data validation passed" 
                    : "Archived data validation failed - state mismatch detected";
                
                _logger.LogInformation("Archive validation for character {CharacterId}: {Result}", 
                    characterId, result.Success ? "PASSED" : "FAILED");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Validation failed: {ex.Message}";
                _logger.LogError(ex, "Failed to validate archived data for character {CharacterId}", characterId);
            }
            
            return result;
        }
        
        private async Task<CharacterArchivingResult> ArchiveCharacterEventsAsync(
            Guid characterId, 
            DateTime cutoffDate, 
            CancellationToken cancellationToken)
        {
            var result = new CharacterArchivingResult { CharacterId = characterId };
            
            try
            {
                // Ensure we have a recent snapshot
                var snapshot = await _snapshotService.CreateSnapshotIfNeededAsync(characterId, cancellationToken);
                if (snapshot == null)
                {
                    // Force snapshot creation for archiving
                    snapshot = await _snapshotService.CreateSnapshotAsync(characterId, cancellationToken);
                }
                
                // Get events older than cutoff but newer than snapshot creation
                var eventsToArchive = await _archiveRepository.GetEventsForArchivingAsync(
                    characterId, cutoffDate, snapshot.CreatedAt.AddDays(-1), cancellationToken);
                
                if (eventsToArchive.Any())
                {
                    // Move events to archive storage
                    await _archiveRepository.ArchiveEventsAsync(characterId, eventsToArchive, cancellationToken);
                    
                    result.EventsArchived = eventsToArchive.Count();
                    result.StorageFreed = eventsToArchive.Sum(e => _eventSerializer.GetEventDataSize(e));
                }
            }
            catch (Exception ex)
            {
                result.HasErrors = true;
                result.Errors.Add($"Character {characterId}: {ex.Message}");
            }
            
            return result;
        }
        
        private List<CompressedEvent> CompressEventSequence(List<IDomainEvent> events)
        {
            // This is a simplified compression algorithm
            // In practice, you'd implement sophisticated compression based on event types
            var compressed = new List<CompressedEvent>();
            
            // Group consecutive events of the same type and compress them
            var groups = events
                .GroupBy(e => e.EventType)
                .Where(g => g.Count() > 1);
            
            foreach (var group in groups)
            {
                var eventList = group.ToList();
                var serializedData = _eventSerializer.SerializeEvents(eventList);
                var compressedData = _eventSerializer.CompressData(serializedData);
                
                var compressedEvent = CompressedEvent.Create(
                    characterId: eventList.First().AggregateId,
                    originalEventId: eventList.First().EventId,
                    eventType: group.Key,
                    compressedData: compressedData,
                    originalSize: serializedData.Length,
                    originalTimestamp: eventList.Min(e => e.OccurredAt)
                );
                
                compressed.Add(compressedEvent);
            }
            
            return compressed;
        }
        
        private Dictionary<DateTime, List<IDomainEvent>> GroupEventsByInterval(
            IEnumerable<IDomainEvent> events, 
            TimeSpan interval)
        {
            return events
                .GroupBy(e => new DateTime((e.OccurredAt.Ticks / interval.Ticks) * interval.Ticks))
                .ToDictionary(g => g.Key, g => g.ToList());
        }
        
        private EventRollup CreateRollupFromEvents(Guid characterId, DateTime interval, List<IDomainEvent> events)
        {
            var eventTypes = events.Select(e => e.EventType).Distinct().ToList();
            var aggregatedData = JsonSerializer.Serialize(new
            {
                EventTypes = eventTypes,
                Summary = $"Rollup of {events.Count} events from {interval:yyyy-MM-dd}"
            });
            
            var originalSize = events.Sum(e => _eventSerializer.GetEventDataSize(e));
            var rollupSize = aggregatedData.Length;
            
            return EventRollup.Create(
                characterId: characterId,
                eventType: string.Join(",", eventTypes),
                eventCount: events.Count,
                startTimestamp: interval,
                endTimestamp: interval.Add(TimeSpan.FromDays(1)), // Daily rollups
                aggregatedData: aggregatedData,
                originalEventIds: events.Select(e => e.EventId).ToList(),
                spaceSaved: (int)(originalSize - rollupSize)
            );
        }
        
        private long CalculateStorageSaved(List<IDomainEvent> original, List<CompressedEvent> compressed)
        {
            // Calculate actual storage using serialization service
            var originalSize = original.Sum(e => _eventSerializer.GetEventDataSize(e));
            var compressedSize = compressed.Sum(e => e.CompressedSize);
            return originalSize - compressedSize;
        }
        
        private async Task<Domain.Entities.Characters.Base.Character> ReconstructFromArchivedDataAsync(
            Guid characterId, 
            CancellationToken cancellationToken)
        {
            // This would reconstruct character state using only archived/compressed data
            // Implementation depends on your archive storage format
            throw new NotImplementedException("Archived data reconstruction not implemented");
        }
        
        private bool CompareCharacterStates(
            Domain.Entities.Characters.Base.Character current, 
            Domain.Entities.Characters.Base.Character archived)
        {
            // Compare critical state properties
            return current.Name == archived.Name &&
                   current.Level == archived.Level &&
                   current.Health == archived.Health &&
                   current.Type == archived.Type;
        }
    }
    
    // Supporting classes and configurations
    public class EventArchivingConfiguration
    {
        public int MaxCharactersPerBatch { get; set; } = 50;
        public TimeSpan DefaultArchiveAge { get; set; } = TimeSpan.FromDays(90);
        public int MaxEventsPerCharacter { get; set; } = 10000;
        public bool ValidateAfterArchiving { get; set; } = true;
        public string ArchiveStorageConnectionString { get; set; }
    }
    
    // Result classes - Application layer DTOs for service contracts
    public class EventArchivingResult
    {
        public bool Success { get; set; }
        public int CharactersProcessed { get; set; }
        public int CharactersWithErrors { get; set; }
        public int EventsArchived { get; set; }
        public long StorageFreed { get; set; }
        public List<string> Errors { get; set; } = new();
    }
    
    public class EventCompressionResult
    {
        public Guid CharacterId { get; set; }
        public bool Success { get; set; }
        public int OriginalEventCount { get; set; }
        public int CompressedEventCount { get; set; }
        public double CompressionRatio { get; set; }
        public long StorageSaved { get; set; }
        public string Message { get; set; }
    }
    
    public class EventRollupResult
    {
        public Guid CharacterId { get; set; }
        public bool Success { get; set; }
        public int RollupsCreated { get; set; }
        public string Message { get; set; }
    }
    
    public class ArchiveValidationResult
    {
        public Guid CharacterId { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
    
    public class CharacterArchivingResult
    {
        public Guid CharacterId { get; set; }
        public int EventsArchived { get; set; }
        public long StorageFreed { get; set; }
        public bool HasErrors { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}

// Archive repository interface
public interface IEventArchiveRepository
{
    Task<IEnumerable<Guid>> GetCharactersEligibleForArchivingAsync(DateTime cutoffDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<IDomainEvent>> GetEventsForArchivingAsync(Guid characterId, DateTime cutoffDate, DateTime afterDate, CancellationToken cancellationToken = default);
    Task ArchiveEventsAsync(Guid characterId, IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default);
    Task SaveCompressedEventsAsync(Guid characterId, List<CompressedEvent> compressedEvents, CancellationToken cancellationToken = default);
    Task SaveEventRollupAsync(EventRollup rollup, CancellationToken cancellationToken = default);
    Task<EventStorageStatistics> GetStorageStatisticsAsync(CancellationToken cancellationToken = default);
}