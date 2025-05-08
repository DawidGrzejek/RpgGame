using Microsoft.AspNetCore.SignalR;
using RpgGame.Application.Events;
using RpgGame.Domain.Events.Base;
using RpgGame.Domain.Events.Characters;
using RpgGame.WebApi.Hubs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RpgGame.WebApi.Services
{
    /// <summary>
    /// Service responsible for sending notifications to clients via SignalR.
    /// </summary>
    public class NotificationService : IEventHandler<CharacterLeveledUp>,
                                      IEventHandler<CharacterDied>
    {
        private readonly IHubContext<GameHub> _hubContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationService"/> class.
        /// </summary>
        /// <param name="hubContext">The SignalR hub context for sending notifications.</param>
        public NotificationService(IHubContext<GameHub> hubContext)
        {
            _hubContext = hubContext;
        }

        /// <summary>
        /// Handles the <see cref="CharacterLeveledUp"/> event by sending a notification to the client.
        /// </summary>
        /// <param name="event">The event data for the character leveling up.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task HandleAsync(CharacterLeveledUp @event, CancellationToken cancellationToken = default)
        {
            await SendCharacterNotification(@event.AggregateId, "level-up", new
            {
                CharacterName = @event.CharacterName,
                OldLevel = @event.OldLevel,
                NewLevel = @event.NewLevel,
                Message = $"{@event.CharacterName} leveled up to {@event.NewLevel}!"
            });
        }

        /// <summary>
        /// Handles the <see cref="CharacterDied"/> event by sending a notification to the client.
        /// </summary>
        /// <param name="event">The event data for the character's death.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task HandleAsync(CharacterDied @event, CancellationToken cancellationToken = default)
        {
            await SendCharacterNotification(@event.AggregateId, "character-died", new
            {
                CharacterName = @event.CharacterName,
                Location = @event.Location,
                Message = $"{@event.CharacterName} has been defeated at {@event.Location}!"
            });
        }

        /// <summary>
        /// Sends a notification to the specified character's group and the global game feed.
        /// </summary>
        /// <param name="characterId">The unique identifier of the character.</param>
        /// <param name="eventType">The type of the event (e.g., "level-up", "character-died").</param>
        /// <param name="data">The data to include in the notification.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task SendCharacterNotification(Guid characterId, string eventType, object data)
        {
            // Send to character's group
            await _hubContext.Clients.Group(characterId.ToString())
                .SendAsync("GameEvent", new { Type = eventType, Data = data });

            // Also send to global game feed
            await _hubContext.Clients.All
                .SendAsync("GameFeed", new { Type = eventType, Data = data });
        }
    }
}