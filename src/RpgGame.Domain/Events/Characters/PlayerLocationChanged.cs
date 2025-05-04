using RpgGame.Domain.Events.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Domain.Events.Characters
{
    /// <summary>
    /// Event raised when player location changes
    /// </summary>
    public class PlayerLocationChanged : DomainEventBase
    {
        public string PlayerName { get; }
        public string OldLocation { get; }
        public string NewLocation { get; }

        public PlayerLocationChanged(string playerName, string oldLocation, string newLocation)
        {
            PlayerName = playerName;
            OldLocation = oldLocation;
            NewLocation = newLocation;
        }
    }
}
