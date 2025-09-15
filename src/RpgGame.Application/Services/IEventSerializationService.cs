using System.Collections.Generic;
using RpgGame.Domain.Events.Base;

namespace RpgGame.Application.Services
{
    /// <summary>
    /// Service responsible for serializing domain events for archiving and compression.
    /// Provides DDD-compliant access to event data without violating layer boundaries.
    /// </summary>
    public interface IEventSerializationService
    {
        /// <summary>
        /// Serializes a single domain event to byte array for storage
        /// </summary>
        /// <param name="domainEvent">The domain event to serialize</param>
        /// <returns>Serialized event data as byte array</returns>
        byte[] SerializeEvent(IDomainEvent domainEvent);
        
        /// <summary>
        /// Serializes multiple domain events to byte array for batch storage
        /// </summary>
        /// <param name="domainEvents">The domain events to serialize</param>
        /// <returns>Serialized events data as byte array</returns>
        byte[] SerializeEvents(IEnumerable<IDomainEvent> domainEvents);
        
        /// <summary>
        /// Compresses serialized event data using efficient compression algorithm
        /// </summary>
        /// <param name="serializedData">The serialized data to compress</param>
        /// <returns>Compressed data as byte array</returns>
        byte[] CompressData(byte[] serializedData);
        
        /// <summary>
        /// Decompresses event data back to serialized form
        /// </summary>
        /// <param name="compressedData">The compressed data to decompress</param>
        /// <returns>Decompressed serialized data</returns>
        byte[] DecompressData(byte[] compressedData);
        
        /// <summary>
        /// Deserializes event data back to domain event
        /// </summary>
        /// <param name="serializedData">The serialized event data</param>
        /// <param name="eventType">The type of event to deserialize to</param>
        /// <returns>Reconstructed domain event</returns>
        IDomainEvent DeserializeEvent(byte[] serializedData, string eventType);
        
        /// <summary>
        /// Gets the serialized size of a domain event for storage calculations
        /// </summary>
        /// <param name="domainEvent">The domain event to measure</param>
        /// <returns>Size in bytes when serialized</returns>
        long GetEventDataSize(IDomainEvent domainEvent);
        
        /// <summary>
        /// Gets compression statistics for monitoring and optimization
        /// </summary>
        /// <param name="originalData">Original data before compression</param>
        /// <param name="compressedData">Data after compression</param>
        /// <returns>Compression statistics including ratio and space saved</returns>
        CompressionStatistics GetCompressionStatistics(byte[] originalData, byte[] compressedData);
    }
    
    /// <summary>
    /// Statistics about compression operation
    /// </summary>
    public class CompressionStatistics
    {
        public long OriginalSize { get; set; }
        public long CompressedSize { get; set; }
        public decimal CompressionRatio => OriginalSize > 0 ? (decimal)CompressedSize / OriginalSize : 0;
        public long SpaceSaved => OriginalSize - CompressedSize;
        public decimal SpaceSavedPercentage => OriginalSize > 0 ? (decimal)SpaceSaved / OriginalSize * 100 : 0;
    }
}