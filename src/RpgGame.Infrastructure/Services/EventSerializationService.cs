using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using RpgGame.Application.Services;
using RpgGame.Domain.Events.Base;
using Microsoft.Extensions.Logging;
using System.Text;

namespace RpgGame.Infrastructure.Services
{
    /// <summary>
    /// Infrastructure implementation of event serialization service.
    /// Handles JSON serialization and gzip compression of domain events.
    /// </summary>
    public class EventSerializationService : IEventSerializationService
    {
        private readonly ILogger<EventSerializationService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public EventSerializationService(ILogger<EventSerializationService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false, // Minimize size for storage
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
        }

        public byte[] SerializeEvent(IDomainEvent domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            try
            {
                // Create a serializable representation of the domain event
                var eventData = new SerializableEventData
                {
                    EventId = domainEvent.EventId,
                    EventType = domainEvent.EventType,
                    AggregateId = domainEvent.AggregateId,
                    Version = domainEvent.Version,
                    OccurredAt = domainEvent.OccurredAt,
                    // Serialize the actual event object to capture all properties
                    EventPayload = JsonSerializer.SerializeToElement(domainEvent, domainEvent.GetType(), _jsonOptions)
                };

                var json = JsonSerializer.Serialize(eventData, _jsonOptions);
                return Encoding.UTF8.GetBytes(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to serialize domain event {EventType} with ID {EventId}", 
                    domainEvent.EventType, domainEvent.EventId);
                throw new InvalidOperationException($"Failed to serialize domain event: {ex.Message}", ex);
            }
        }

        public byte[] SerializeEvents(IEnumerable<IDomainEvent> domainEvents)
        {
            if (domainEvents == null)
                throw new ArgumentNullException(nameof(domainEvents));

            var eventList = domainEvents.ToList();
            if (!eventList.Any())
                return Array.Empty<byte>();

            try
            {
                var serializedEvents = eventList.Select(SerializeEvent).ToList();
                var batchData = new SerializableEventBatch
                {
                    EventCount = eventList.Count,
                    BatchId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    Events = serializedEvents.Select(data => Convert.ToBase64String(data)).ToList()
                };

                var json = JsonSerializer.Serialize(batchData, _jsonOptions);
                return Encoding.UTF8.GetBytes(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to serialize {EventCount} domain events", eventList.Count);
                throw new InvalidOperationException($"Failed to serialize domain events: {ex.Message}", ex);
            }
        }

        public byte[] CompressData(byte[] serializedData)
        {
            if (serializedData == null || serializedData.Length == 0)
                return Array.Empty<byte>();

            try
            {
                using var output = new MemoryStream();
                using (var gzip = new GZipStream(output, CompressionLevel.Optimal))
                {
                    gzip.Write(serializedData, 0, serializedData.Length);
                }
                
                var compressed = output.ToArray();
                _logger.LogDebug("Compressed {OriginalSize} bytes to {CompressedSize} bytes (ratio: {Ratio:P1})", 
                    serializedData.Length, compressed.Length, (double)compressed.Length / serializedData.Length);
                
                return compressed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to compress {DataSize} bytes", serializedData.Length);
                throw new InvalidOperationException($"Failed to compress data: {ex.Message}", ex);
            }
        }

        public byte[] DecompressData(byte[] compressedData)
        {
            if (compressedData == null || compressedData.Length == 0)
                return Array.Empty<byte>();

            try
            {
                using var input = new MemoryStream(compressedData);
                using var gzip = new GZipStream(input, CompressionMode.Decompress);
                using var output = new MemoryStream();
                
                gzip.CopyTo(output);
                return output.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to decompress {DataSize} bytes", compressedData.Length);
                throw new InvalidOperationException($"Failed to decompress data: {ex.Message}", ex);
            }
        }

        public IDomainEvent DeserializeEvent(byte[] serializedData, string eventType)
        {
            if (serializedData == null || serializedData.Length == 0)
                throw new ArgumentNullException(nameof(serializedData));
            
            if (string.IsNullOrWhiteSpace(eventType))
                throw new ArgumentException("Event type cannot be null or empty", nameof(eventType));

            try
            {
                var json = Encoding.UTF8.GetString(serializedData);
                var eventData = JsonSerializer.Deserialize<SerializableEventData>(json, _jsonOptions);
                
                if (eventData == null)
                    throw new InvalidOperationException("Deserialized event data is null");

                // This is a simplified approach - in production, you'd use proper type resolution
                // based on the event type string to recreate the exact domain event type
                throw new NotImplementedException(
                    "Event deserialization requires type resolution mapping. " +
                    "Implement based on your domain event type registration strategy.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize event of type {EventType}", eventType);
                throw new InvalidOperationException($"Failed to deserialize event: {ex.Message}", ex);
            }
        }

        public long GetEventDataSize(IDomainEvent domainEvent)
        {
            if (domainEvent == null)
                return 0;

            try
            {
                var serialized = SerializeEvent(domainEvent);
                return serialized.Length;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to calculate size for event {EventType}, using approximation", 
                    domainEvent.EventType);
                
                // Fallback to approximation if serialization fails
                return domainEvent.EventType.Length + 100; // Rough estimate
            }
        }

        public CompressionStatistics GetCompressionStatistics(byte[] originalData, byte[] compressedData)
        {
            return new CompressionStatistics
            {
                OriginalSize = originalData?.Length ?? 0,
                CompressedSize = compressedData?.Length ?? 0
            };
        }

        // Helper classes for serialization
        private class SerializableEventData
        {
            public Guid EventId { get; set; }
            public string EventType { get; set; }
            public Guid AggregateId { get; set; }
            public int Version { get; set; }
            public DateTime OccurredAt { get; set; }
            public JsonElement EventPayload { get; set; }
        }

        private class SerializableEventBatch
        {
            public int EventCount { get; set; }
            public Guid BatchId { get; set; }
            public DateTime CreatedAt { get; set; }
            public List<string> Events { get; set; } = new();
        }
    }
}