using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace RpgGame.WebApi.Hubs
{
    /// <summary>
    /// SignalR hub for managing character group interactions in the RPG game.
    /// </summary>
    public class GameHub : Hub
    {
        /// <summary>
        /// Adds the current connection to a group associated with the specified character ID.
        /// </summary>
        /// <param name="characterId">The unique identifier of the character.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task JoinCharacterGroup(Guid characterId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, characterId.ToString());
        }

        /// <summary>
        /// Removes the current connection from a group associated with the specified character ID.
        /// </summary>
        /// <param name="characterId">The unique identifier of the character.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task LeaveCharacterGroup(Guid characterId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, characterId.ToString());
        }
    }
}