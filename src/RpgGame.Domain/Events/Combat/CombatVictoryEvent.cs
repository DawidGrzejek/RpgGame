using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Domain.Events.Base;

namespace RpgGame.Domain.Events.Combat
{
    public class CombatVictoryEvent : DomainEventBase
    {
        public string PlayerName { get; }
        public string EnemyName { get; }
        public int ExperienceGained { get; }
        public IReadOnlyList<string> ItemsDropped { get; }

        public CombatVictoryEvent(
            Guid aggregateId,
            string playerName,
            string enemyName,
            int experienceGained,
            IReadOnlyList<string> itemsDropped) : base(aggregateId)
        {
            PlayerName = playerName;
            EnemyName = enemyName;
            ExperienceGained = experienceGained;
            ItemsDropped = itemsDropped;
        }
    }
}
