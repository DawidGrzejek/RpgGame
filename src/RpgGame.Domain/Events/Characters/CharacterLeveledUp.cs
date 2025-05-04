using RpgGame.Domain.Events.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Domain.Events.Characters
{
    /// <summary>
    /// Event raised when character levels up
    /// </summary>
    public class CharacterLeveledUp : DomainEventBase
    {
        public string CharacterName { get; }
        public int OldLevel { get; }
        public int NewLevel { get; }
        public int HealthIncrease { get; }
        public int StrengthIncrease { get; }
        public int DefenseIncrease { get; }

        public CharacterLeveledUp(string characterName, int oldLevel, int newLevel, int healthIncrease, int strengthIncrease, int defenseIncrease)
        {
            CharacterName = characterName;
            OldLevel = oldLevel;
            NewLevel = newLevel;
            HealthIncrease = healthIncrease;
            StrengthIncrease = strengthIncrease;
            DefenseIncrease = defenseIncrease;
        }
    }
}
