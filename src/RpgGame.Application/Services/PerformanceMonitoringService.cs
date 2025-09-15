using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RpgGame.Application.Services
{
    /// <summary>
    /// Service for monitoring event sourcing performance and triggering optimizations
    /// </summary>
    public interface IPerformanceMonitoringService
    {
        /// <summary>
        /// Records performance metrics for character reconstruction
        /// </summary>
        Task RecordCharacterReconstructionMetricsAsync(Guid characterId, TimeSpan duration, int eventCount, bool usedSnapshot);
        
        /// <summary>
        /// Gets performance statistics for monitoring dashboards
        /// </summary>
        Task<PerformanceStatistics> GetPerformanceStatisticsAsync(TimeSpan period);
        
        /// <summary>
        /// Identifies characters that would benefit from snapshots or optimization
        /// </summary>
        Task<IEnumerable<OptimizationRecommendation>> GetOptimizationRecommendationsAsync();
        
        /// <summary>
        /// Gets real-time performance metrics
        /// </summary>
        RealtimeMetrics GetRealtimeMetrics();
    }
    
    /// <summary>
    /// Background service that monitors performance and triggers optimizations
    /// </summary>
    public class PerformanceMonitoringService : BackgroundService, IPerformanceMonitoringService
    {
        private readonly ISnapshotService _snapshotService;
        private readonly IEventArchivingService _archivingService;
        private readonly ILogger<PerformanceMonitoringService> _logger;
        private readonly PerformanceMonitoringOptions _options;
        
        private readonly Dictionary<Guid, List<PerformanceMetric>> _recentMetrics = new();
        private readonly object _metricsLock = new object();
        private readonly RealtimeMetrics _realtimeMetrics = new();
        
        public PerformanceMonitoringService(
            ISnapshotService snapshotService,
            IEventArchivingService archivingService,
            ILogger<PerformanceMonitoringService> logger,
            IOptions<PerformanceMonitoringOptions> options)
        {
            _snapshotService = snapshotService ?? throw new ArgumentNullException(nameof(snapshotService));
            _archivingService = archivingService ?? throw new ArgumentNullException(nameof(archivingService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }
        
        public async Task RecordCharacterReconstructionMetricsAsync(
            Guid characterId, 
            TimeSpan duration, 
            int eventCount, 
            bool usedSnapshot)
        {
            var metric = new PerformanceMetric
            {
                CharacterId = characterId,
                Timestamp = DateTime.UtcNow,
                Duration = duration,
                EventCount = eventCount,
                UsedSnapshot = usedSnapshot,
                PerformanceScore = CalculatePerformanceScore(duration, eventCount, usedSnapshot)
            };
            
            lock (_metricsLock)
            {
                if (!_recentMetrics.ContainsKey(characterId))
                {
                    _recentMetrics[characterId] = new List<PerformanceMetric>();
                }
                
                _recentMetrics[characterId].Add(metric);
                
                // Keep only recent metrics (last 100 per character)
                if (_recentMetrics[characterId].Count > 100)
                {
                    _recentMetrics[characterId].RemoveAt(0);
                }
                
                // Update realtime metrics
                _realtimeMetrics.TotalReconstructions++;
                if (usedSnapshot)
                {
                    _realtimeMetrics.ReconstructionsUsingSnapshots++;
                    _realtimeMetrics.AverageSnapshotReconstructionTime = CalculateMovingAverage(
                        _realtimeMetrics.AverageSnapshotReconstructionTime, duration.TotalMilliseconds);
                }
                else
                {
                    _realtimeMetrics.ReconstructionsUsingFullEvents++;
                    _realtimeMetrics.AverageFullReconstructionTime = CalculateMovingAverage(
                        _realtimeMetrics.AverageFullReconstructionTime, duration.TotalMilliseconds);
                }
                
                // Log performance issues
                if (duration.TotalSeconds > _options.SlowReconstructionThresholdSeconds)
                {
                    _logger.LogWarning("Slow character reconstruction detected: {CharacterId} took {Duration}ms with {EventCount} events (snapshot: {UsedSnapshot})", 
                        characterId, duration.TotalMilliseconds, eventCount, usedSnapshot);
                }
            }
            
            // Async optimization trigger
            if (ShouldTriggerOptimization(metric))
            {
                _ = Task.Run(() => TriggerOptimizationAsync(characterId));
            }
        }
        
        public async Task<PerformanceStatistics> GetPerformanceStatisticsAsync(TimeSpan period)
        {
            var cutoffTime = DateTime.UtcNow - period;
            var stats = new PerformanceStatistics { Period = period };
            
            lock (_metricsLock)
            {
                var relevantMetrics = _recentMetrics.Values
                    .SelectMany(m => m)
                    .Where(m => m.Timestamp >= cutoffTime)
                    .ToList();
                
                if (relevantMetrics.Any())
                {
                    stats.TotalReconstructions = relevantMetrics.Count;
                    stats.ReconstructionsUsingSnapshots = relevantMetrics.Count(m => m.UsedSnapshot);
                    stats.AverageReconstructionTime = TimeSpan.FromMilliseconds(relevantMetrics.Average(m => m.Duration.TotalMilliseconds));
                    stats.MaxReconstructionTime = TimeSpan.FromMilliseconds(relevantMetrics.Max(m => m.Duration.TotalMilliseconds));
                    stats.AverageEventCount = (int)relevantMetrics.Average(m => m.EventCount);
                    stats.MaxEventCount = relevantMetrics.Max(m => m.EventCount);
                    
                    var snapshotMetrics = relevantMetrics.Where(m => m.UsedSnapshot).ToList();
                    var fullEventMetrics = relevantMetrics.Where(m => !m.UsedSnapshot).ToList();
                    
                    if (snapshotMetrics.Any())
                    {
                        stats.AverageSnapshotReconstructionTime = TimeSpan.FromMilliseconds(snapshotMetrics.Average(m => m.Duration.TotalMilliseconds));
                    }
                    
                    if (fullEventMetrics.Any())
                    {
                        stats.AverageFullEventReconstructionTime = TimeSpan.FromMilliseconds(fullEventMetrics.Average(m => m.Duration.TotalMilliseconds));
                    }
                    
                    stats.PerformanceImprovement = CalculatePerformanceImprovement(snapshotMetrics, fullEventMetrics);
                    stats.SlowReconstructions = relevantMetrics.Count(m => m.Duration.TotalSeconds > _options.SlowReconstructionThresholdSeconds);
                }
            }
            
            return stats;
        }
        
        public async Task<IEnumerable<OptimizationRecommendation>> GetOptimizationRecommendationsAsync()
        {
            var recommendations = new List<OptimizationRecommendation>();
            
            lock (_metricsLock)
            {
                foreach (var kvp in _recentMetrics)
                {
                    var characterId = kvp.Key;
                    var metrics = kvp.Value.TakeLast(20).ToList(); // Last 20 reconstructions
                    
                    if (!metrics.Any()) continue;
                    
                    var avgDuration = metrics.Average(m => m.Duration.TotalMilliseconds);
                    var avgEventCount = metrics.Average(m => m.EventCount);
                    var snapshotUsage = (double)metrics.Count(m => m.UsedSnapshot) / metrics.Count;
                    
                    // Recommend snapshot if reconstruction is slow and no snapshots are being used
                    if (avgDuration > _options.SlowReconstructionThresholdSeconds * 1000 && snapshotUsage < 0.5)
                    {
                        recommendations.Add(new OptimizationRecommendation
                        {
                            CharacterId = characterId,
                            Type = OptimizationType.CreateSnapshot,
                            Priority = avgDuration > _options.SlowReconstructionThresholdSeconds * 2000 ? Priority.High : Priority.Medium,
                            Reason = $"Slow reconstruction (avg {avgDuration:F0}ms) with {avgEventCount:F0} events",
                            ExpectedImprovement = EstimateSnapshotImprovement(avgDuration, avgEventCount)
                        });
                    }
                    
                    // Recommend archiving if event count is very high
                    if (avgEventCount > _options.HighEventCountThreshold)
                    {
                        recommendations.Add(new OptimizationRecommendation
                        {
                            CharacterId = characterId,
                            Type = OptimizationType.ArchiveOldEvents,
                            Priority = avgEventCount > _options.HighEventCountThreshold * 2 ? Priority.High : Priority.Medium,
                            Reason = $"High event count ({avgEventCount:F0} events)",
                            ExpectedImprovement = EstimateArchivingImprovement(avgEventCount)
                        });
                    }
                    
                    // Recommend compression if event history shows redundancy
                    if (avgEventCount > _options.CompressionThreshold && snapshotUsage > 0.8)
                    {
                        recommendations.Add(new OptimizationRecommendation
                        {
                            CharacterId = characterId,
                            Type = OptimizationType.CompressEventHistory,
                            Priority = Priority.Low,
                            Reason = "High event count with good snapshot coverage",
                            ExpectedImprovement = "Reduce storage by 30-50%"
                        });
                    }
                }
            }
            
            return recommendations.OrderByDescending(r => r.Priority).ThenByDescending(r => r.CharacterId);
        }
        
        public RealtimeMetrics GetRealtimeMetrics()
        {
            lock (_metricsLock)
            {
                return new RealtimeMetrics
                {
                    TotalReconstructions = _realtimeMetrics.TotalReconstructions,
                    ReconstructionsUsingSnapshots = _realtimeMetrics.ReconstructionsUsingSnapshots,
                    ReconstructionsUsingFullEvents = _realtimeMetrics.ReconstructionsUsingFullEvents,
                    AverageSnapshotReconstructionTime = _realtimeMetrics.AverageSnapshotReconstructionTime,
                    AverageFullReconstructionTime = _realtimeMetrics.AverageFullReconstructionTime,
                    LastUpdated = DateTime.UtcNow
                };
            }
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Performance monitoring service started");
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await PerformPeriodicMaintenanceAsync(stoppingToken);
                    await Task.Delay(TimeSpan.FromMinutes(_options.MaintenanceIntervalMinutes), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancellation is requested
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during performance monitoring maintenance");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Brief delay before retry
                }
            }
            
            _logger.LogInformation("Performance monitoring service stopped");
        }
        
        private async Task PerformPeriodicMaintenanceAsync(CancellationToken cancellationToken)
        {
            // Process pending snapshots
            await _snapshotService.ProcessPendingSnapshotsAsync(cancellationToken);
            
            // Clean up old snapshots
            await _snapshotService.CleanupOldSnapshotsAsync(cancellationToken);
            
            // Archive old events if enabled
            if (_options.EnableAutoArchiving)
            {
                await _archivingService.ArchiveOldEventsAsync(_options.AutoArchiveAge, cancellationToken);
            }
            
            // Clean up old metrics
            CleanupOldMetrics();
            
            _logger.LogDebug("Performance monitoring maintenance completed");
        }
        
        private void CleanupOldMetrics()
        {
            var cutoffTime = DateTime.UtcNow - TimeSpan.FromHours(_options.MetricsRetentionHours);
            
            lock (_metricsLock)
            {
                var keysToRemove = new List<Guid>();
                
                foreach (var kvp in _recentMetrics)
                {
                    kvp.Value.RemoveAll(m => m.Timestamp < cutoffTime);
                    if (!kvp.Value.Any())
                    {
                        keysToRemove.Add(kvp.Key);
                    }
                }
                
                foreach (var key in keysToRemove)
                {
                    _recentMetrics.Remove(key);
                }
            }
        }
        
        private bool ShouldTriggerOptimization(PerformanceMetric metric)
        {
            return metric.Duration.TotalSeconds > _options.SlowReconstructionThresholdSeconds && 
                   !metric.UsedSnapshot &&
                   metric.EventCount > _options.SnapshotRecommendationThreshold;
        }
        
        private async Task TriggerOptimizationAsync(Guid characterId)
        {
            try
            {
                await _snapshotService.CreateSnapshotIfNeededAsync(characterId);
                _logger.LogInformation("Triggered optimization for character {CharacterId}", characterId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to trigger optimization for character {CharacterId}", characterId);
            }
        }
        
        private double CalculatePerformanceScore(TimeSpan duration, int eventCount, bool usedSnapshot)
        {
            // Simple scoring algorithm - lower is better
            var baseScore = duration.TotalMilliseconds / 10.0; // Base score from duration
            var eventPenalty = Math.Log(eventCount + 1) * 5; // Logarithmic penalty for event count
            var snapshotBonus = usedSnapshot ? -20 : 0; // Bonus for using snapshots
            
            return Math.Max(0, baseScore + eventPenalty + snapshotBonus);
        }
        
        private double CalculateMovingAverage(double currentAverage, double newValue, int windowSize = 100)
        {
            return currentAverage == 0 ? newValue : ((currentAverage * (windowSize - 1)) + newValue) / windowSize;
        }
        
        private double CalculatePerformanceImprovement(
            List<PerformanceMetric> snapshotMetrics, 
            List<PerformanceMetric> fullEventMetrics)
        {
            if (!snapshotMetrics.Any() || !fullEventMetrics.Any()) return 0;
            
            var avgSnapshotTime = snapshotMetrics.Average(m => m.Duration.TotalMilliseconds);
            var avgFullEventTime = fullEventMetrics.Average(m => m.Duration.TotalMilliseconds);
            
            return avgFullEventTime > 0 ? ((avgFullEventTime - avgSnapshotTime) / avgFullEventTime) * 100 : 0;
        }
        
        private string EstimateSnapshotImprovement(double avgDuration, double avgEventCount)
        {
            var estimatedImprovement = Math.Min(90, avgEventCount / 100 * 60); // Rough estimate
            return $"{estimatedImprovement:F0}% faster reconstruction";
        }
        
        private string EstimateArchivingImprovement(double avgEventCount)
        {
            var storageReduction = Math.Min(80, (avgEventCount - 1000) / 100 * 10);
            return $"{storageReduction:F0}% storage reduction";
        }
    }
    
    // Configuration and data classes
    public class PerformanceMonitoringOptions
    {
        public double SlowReconstructionThresholdSeconds { get; set; } = 1.0;
        public int HighEventCountThreshold { get; set; } = 5000;
        public int CompressionThreshold { get; set; } = 10000;
        public int SnapshotRecommendationThreshold { get; set; } = 1000;
        public int MaintenanceIntervalMinutes { get; set; } = 30;
        public int MetricsRetentionHours { get; set; } = 24;
        public bool EnableAutoArchiving { get; set; } = true;
        public TimeSpan AutoArchiveAge { get; set; } = TimeSpan.FromDays(90);
    }
    
    public class PerformanceMetric
    {
        public Guid CharacterId { get; set; }
        public DateTime Timestamp { get; set; }
        public TimeSpan Duration { get; set; }
        public int EventCount { get; set; }
        public bool UsedSnapshot { get; set; }
        public double PerformanceScore { get; set; }
    }
    
    public class PerformanceStatistics
    {
        public TimeSpan Period { get; set; }
        public int TotalReconstructions { get; set; }
        public int ReconstructionsUsingSnapshots { get; set; }
        public TimeSpan AverageReconstructionTime { get; set; }
        public TimeSpan MaxReconstructionTime { get; set; }
        public int AverageEventCount { get; set; }
        public int MaxEventCount { get; set; }
        public TimeSpan AverageSnapshotReconstructionTime { get; set; }
        public TimeSpan AverageFullEventReconstructionTime { get; set; }
        public double PerformanceImprovement { get; set; }
        public int SlowReconstructions { get; set; }
    }
    
    public class RealtimeMetrics
    {
        public int TotalReconstructions { get; set; }
        public int ReconstructionsUsingSnapshots { get; set; }
        public int ReconstructionsUsingFullEvents { get; set; }
        public double AverageSnapshotReconstructionTime { get; set; }
        public double AverageFullReconstructionTime { get; set; }
        public DateTime LastUpdated { get; set; }
    }
    
    public class OptimizationRecommendation
    {
        public Guid CharacterId { get; set; }
        public OptimizationType Type { get; set; }
        public Priority Priority { get; set; }
        public string Reason { get; set; }
        public string ExpectedImprovement { get; set; }
    }
    
    public enum OptimizationType
    {
        CreateSnapshot,
        ArchiveOldEvents,
        CompressEventHistory
    }
    
    public enum Priority
    {
        Low,
        Medium,
        High
    }
}