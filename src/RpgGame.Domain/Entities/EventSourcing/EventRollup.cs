using System;
using System.Collections.Generic;
using RpgGame.Domain.Base;

namespace RpgGame.Domain.Entities.EventSourcing
{
    /// <summary>
    /// Represents a rollup of multiple events into summarized form for long-term storage
    /// Used when many similar events can be aggregated (e.g., 100 small damage events -> 1 total damage rollup)
    /// </summary>
    public class EventRollup : DomainEntity
    {
        /// <summary>
        /// The Character ID this rollup belongs to
        /// </summary>
        public Guid CharacterId { get; private set; }
        
        /// <summary>
        /// Type of events that were rolled up (e.g., "DamageDealt", "ExperienceGained")
        /// </summary>
        public string EventType { get; private set; }
        
        /// <summary>
        /// Number of original events that were consolidated into this rollup
        /// </summary>
        public int EventCount { get; private set; }
        
        /// <summary>
        /// First event timestamp in the rollup period
        /// </summary>
        public DateTime StartTimestamp { get; private set; }
        
        /// <summary>
        /// Last event timestamp in the rollup period
        /// </summary>
        public DateTime EndTimestamp { get; private set; }
        
        /// <summary>
        /// Aggregated data as JSON (e.g., total damage, total experience, etc.)
        /// </summary>
        public string AggregatedData { get; private set; }
        
        /// <summary>
        /// List of original event IDs that were rolled up for audit purposes
        /// </summary>
        public string OriginalEventIds { get; private set; }
        
        /// <summary>
        /// When this rollup was created
        /// </summary>
        public DateTime CreatedAt { get; private set; }
        
        /// <summary>
        /// Size reduction achieved by rollup (original bytes - rollup bytes)
        /// </summary>
        public int SpaceSaved { get; private set; }

        // EF Core constructor
        private EventRollup() { }

        /// <summary>
        /// Creates a new event rollup
        /// </summary>
        public static EventRollup Create(
            Guid characterId,
            string eventType,
            int eventCount,
            DateTime startTimestamp,
            DateTime endTimestamp,
            string aggregatedData,
            List<Guid> originalEventIds,
            int spaceSaved)
        {
            var rollup = new EventRollup
            {
                Id = Guid.NewGuid(),
                CharacterId = characterId,
                EventType = eventType,
                EventCount = eventCount,
                StartTimestamp = startTimestamp,
                EndTimestamp = endTimestamp,
                AggregatedData = aggregatedData,
                OriginalEventIds = string.Join(",", originalEventIds),
                CreatedAt = DateTime.UtcNow,
                SpaceSaved = spaceSaved
            };

            return rollup;
        }
        
        /// <summary>
        /// Gets the duration covered by this rollup
        /// </summary>
        public TimeSpan GetDuration()
        {
            return EndTimestamp - StartTimestamp;
        }
        
        /// <summary>
        /// Gets the original event IDs as a list
        /// </summary>
        public List<Guid> GetOriginalEventIdsList()
        {
            if (string.IsNullOrEmpty(OriginalEventIds))
                return new List<Guid>();
                
            var ids = new List<Guid>();
            foreach (var idString in OriginalEventIds.Split(','))
            {
                if (Guid.TryParse(idString, out var id))
                    ids.Add(id);
            }
            return ids;
        }
    }
}