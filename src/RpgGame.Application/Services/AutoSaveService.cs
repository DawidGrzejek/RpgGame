using RpgGame.Application.Services;
using RpgGame.Domain.Entities.Characters.Base;
using RpgGame.Domain.Interfaces.World;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RpgGame.Application.Services
{
    public class AutoSaveService : IDisposable
    {
        private readonly GameSaveService _gameSaveService;
        private Timer _autoSaveTimer;
        private const int DEFAULT_AUTOSAVE_INTERVAL = 5; // minutes
        private readonly string _autoSaveName = "AutoSave";
        private bool _disposed = false;

        private Character _player;
        private ILocation _currentLocation;

        public bool IsEnabled { get; private set; }
        public int IntervalMinutes { get; private set; }

        public AutoSaveService(GameSaveService gameSaveService)
        {
            _gameSaveService = gameSaveService ?? throw new ArgumentNullException(nameof(gameSaveService));
            IntervalMinutes = DEFAULT_AUTOSAVE_INTERVAL;
            IsEnabled = true;
        }

        public void Initialize(Character player, ILocation currentLocation)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _currentLocation = currentLocation ?? throw new ArgumentNullException(nameof(currentLocation));

            // Start autosave timer
            StartAutoSaveTimer();
        }

        public void UpdateGameState(Character player, ILocation currentLocation)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _currentLocation = currentLocation ?? throw new ArgumentNullException(nameof(currentLocation));
        }

        public void SetInterval(int minutes)
        {
            if (minutes <= 0)
                throw new ArgumentException("Interval must be greater than zero", nameof(minutes));

            IntervalMinutes = minutes;

            // Reset timer with new interval
            if (IsEnabled)
            {
                StopAutoSaveTimer();
                StartAutoSaveTimer();
            }
        }

        public void Enable()
        {
            if (!IsEnabled)
            {
                IsEnabled = true;
                StartAutoSaveTimer();
            }
        }

        public void Disable()
        {
            if (IsEnabled)
            {
                IsEnabled = false;
                StopAutoSaveTimer();
            }
        }

        private void StartAutoSaveTimer()
        {
            // Convert minutes to milliseconds
            int intervalMs = IntervalMinutes * 60 * 1000;

            // Create and start the timer
            _autoSaveTimer = new Timer(
                PerformAutoSave,      // Callback
                null,                 // State object (not used)
                intervalMs,           // First interval
                intervalMs);          // Subsequent intervals

            Console.WriteLine($"AutoSave enabled. Game will save automatically every {IntervalMinutes} minutes.");
        }

        private void StopAutoSaveTimer()
        {
            _autoSaveTimer?.Dispose();
            _autoSaveTimer = null;

            Console.WriteLine("AutoSave disabled.");
        }

        private void PerformAutoSave(object state)
        {
            try
            {
                if (_player != null && _currentLocation != null)
                {
                    // Skip autosave if player is dead
                    if (!_player.IsAlive)
                        return;

                    // Perform the save
                    bool success = _gameSaveService.SaveGame(_autoSaveName, _player, _currentLocation);

                    if (success)
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Auto-saved game successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Auto-save failed.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during auto-save: {ex.Message}");
            }
        }

        public void TriggerImmediateSave()
        {
            PerformAutoSave(null);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    StopAutoSaveTimer();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AutoSaveService()
        {
            Dispose(false);
        }
    }
}