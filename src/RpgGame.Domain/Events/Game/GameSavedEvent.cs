using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Domain.Events.Base;

namespace RpgGame.Domain.Events.Game
{
    public class GameSavedEvent : DomainEventBase
    {
        public string SaveName { get; }
        public string PlayerName { get; }
        public int PlayerLevel { get; }
        public string LocationName { get; }
        public DateTime SaveDateTime { get; }

        public GameSavedEvent(
            Guid aggregateId,
            int version,
            string saveName,
            string playerName,
            int playerLevel,
            string locationName,
            DateTime saveDateTime) : base(aggregateId, version)
        {
            SaveName = saveName;
            PlayerName = playerName;
            PlayerLevel = playerLevel;
            LocationName = locationName;
            SaveDateTime = saveDateTime;
        }
    }
}
