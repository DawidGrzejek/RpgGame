using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RpgGame.Application.Services;
using RpgGame.Domain.Base;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Events.Base;

namespace RpgGame.Application.Events
{
    /// <summary>
    /// Enhanced event sourcing service that uses snapshots for performance optimization.
    /// This service automatically falls back to the original full-event reconstruction if snapshots fail.
    /// </summary>
    public class OptimizedEventSourcingService : IEventSourcingService
    {
        private readonly ISnapshotService _snapshotService;
        private readonly EventSourcingService _fallbackService;
        private readonly ILogger<OptimizedEventSourcingService> _logger;
        
        public OptimizedEventSourcingService(
            ISnapshotService snapshotService,
            EventSourcingService fallbackService,
            ILogger<OptimizedEventSourcingService> logger)
        {
            _snapshotService = snapshotService ?? throw new ArgumentNullException(nameof(snapshotService));
            _fallbackService = fallbackService ?? throw new ArgumentNullException(nameof(fallbackService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<T> GetByIdAsync<T>(Guid id) where T : class
        {
            // For Character entities, use optimized snapshot-based reconstruction
            if (typeof(Character).IsAssignableFrom(typeof(T)))
            {
                try
                {
                    var character = await _snapshotService.GetCharacterByIdAsync(id);
                    return character as T;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Snapshot-based reconstruction failed for character {CharacterId}, falling back to full event reconstruction", id);
                    // Fallback to original event sourcing
                    return await _fallbackService.GetByIdAsync<T>(id);
                }
            }
            
            // For other aggregate types, use the original event sourcing
            return await _fallbackService.GetByIdAsync<T>(id);
        }
        
        public async Task SaveAsync<T>(T aggregate) where T : DomainEntity
        {
            // Save using the original event sourcing service
            await _fallbackService.SaveAsync(aggregate);
            
            // For Character entities, trigger snapshot evaluation asynchronously
            if (aggregate is Character character)
            {
                // Fire-and-forget snapshot creation check
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _snapshotService.CreateSnapshotIfNeededAsync(character.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to evaluate snapshot needs for character {CharacterId}", character.Id);
                    }
                });
            }
        }
    }
    
    /// <summary>
    /// Factory service to create the appropriate event sourcing service based on configuration
    /// </summary>
    public class EventSourcingServiceFactory
    {
        private readonly ISnapshotService _snapshotService;
        private readonly EventSourcingService _originalService;
        private readonly ILogger<OptimizedEventSourcingService> _logger;
        private readonly bool _snapshotsEnabled;
        
        public EventSourcingServiceFactory(
            ISnapshotService snapshotService,
            EventSourcingService originalService,
            ILogger<OptimizedEventSourcingService> logger,
            bool snapshotsEnabled = true)
        {
            _snapshotService = snapshotService;
            _originalService = originalService ?? throw new ArgumentNullException(nameof(originalService));
            _logger = logger;
            _snapshotsEnabled = snapshotsEnabled;
        }
        
        public IEventSourcingService CreateService()
        {
            if (_snapshotsEnabled && _snapshotService != null)
            {
                return new OptimizedEventSourcingService(_snapshotService, _originalService, _logger);
            }
            
            return _originalService;
        }
    }
}