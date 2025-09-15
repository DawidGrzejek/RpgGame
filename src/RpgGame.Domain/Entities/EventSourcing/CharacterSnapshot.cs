using System;
using System.Collections.Generic;
using RpgGame.Domain.Base;
using RpgGame.Domain.Enums;
using RpgGame.Domain.ValueObjects;

namespace RpgGame.Domain.Entities.EventSourcing
{
    /// <summary>
    /// Represents a point-in-time snapshot of a Character's state for performance optimization in event sourcing.
    /// Snapshots prevent the need to replay millions of events for long-running characters.
    /// </summary>
    public class CharacterSnapshot : DomainEntity
    {
        /// <summary>
        /// The Character ID this snapshot represents
        /// </summary>
        public Guid CharacterId { get; private set; }
        
        /// <summary>
        /// The event version this snapshot captures up to
        /// </summary>
        public int EventVersion { get; private set; }
        
        /// <summary>
        /// Total number of events that were used to create this snapshot
        /// </summary>
        public int TotalEventCount { get; private set; }
        
        /// <summary>
        /// When this snapshot was created
        /// </summary>
        public DateTime CreatedAt { get; private set; }
        
        /// <summary>
        /// Serialized character state as JSON for flexibility
        /// </summary>
        public string SerializedState { get; private set; }
        
        /// <summary>
        /// Character name for quick lookups and debugging
        /// </summary>
        public string CharacterName { get; private set; }
        
        /// <summary>
        /// Character level at time of snapshot for indexing/querying
        /// </summary>
        public int CharacterLevel { get; private set; }
        
        /// <summary>
        /// Character type for indexing/querying
        /// </summary>
        public CharacterType CharacterType { get; private set; }
        
        /// <summary>
        /// Indicates if this is the latest snapshot for the character
        /// </summary>
        public bool IsLatest { get; private set; }
        
        /// <summary>
        /// Size of the serialized state in bytes for monitoring
        /// </summary>
        public int StateSize { get; private set; }
        
        /// <summary>
        /// Performance metrics - time taken to create this snapshot
        /// </summary>
        public TimeSpan CreationDuration { get; private set; }

        // EF Core constructor
        private CharacterSnapshot() { }

        /// <summary>
        /// Creates a new character snapshot
        /// </summary>
        public static CharacterSnapshot Create(
            Guid characterId,
            string characterName,
            int characterLevel,
            CharacterType characterType,
            int eventVersion,
            int totalEventCount,
            string serializedState,
            TimeSpan creationDuration)
        {
            var snapshot = new CharacterSnapshot
            {
                Id = Guid.NewGuid(),
                CharacterId = characterId,
                CharacterName = characterName,
                CharacterLevel = characterLevel,
                CharacterType = characterType,
                EventVersion = eventVersion,
                TotalEventCount = totalEventCount,
                SerializedState = serializedState,
                StateSize = System.Text.Encoding.UTF8.GetByteCount(serializedState),
                CreatedAt = DateTime.UtcNow,
                IsLatest = true,
                CreationDuration = creationDuration
            };

            return snapshot;
        }
        
        /// <summary>
        /// Marks this snapshot as no longer being the latest
        /// </summary>
        public void MarkAsOld()
        {
            IsLatest = false;
        }
        
        /// <summary>
        /// Gets basic snapshot info for logging and monitoring
        /// </summary>
        public SnapshotInfo GetInfo()
        {
            return new SnapshotInfo(
                CharacterId,
                CharacterName,
                EventVersion,
                TotalEventCount,
                CreatedAt,
                StateSize,
                CreationDuration
            );
        }
    }
    
    /// <summary>
    /// Value object containing snapshot metadata for monitoring and reporting
    /// </summary>
    public record SnapshotInfo(
        Guid CharacterId,
        string CharacterName,
        int EventVersion,
        int TotalEventCount,
        DateTime CreatedAt,
        int StateSize,
        TimeSpan CreationDuration
    );
}