using RpgGame.Domain.Events.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Domain.Events.Characters
{
    /// <summary>
    /// Event raised when player gains experience
    /// </summary>
    public class PlayerGainedExperience : DomainEventBase
    {
        public string PlayerName { get; }
        public int ExperienceGained { get; }
        public int TotalExperience { get; }
        public int ExperienceToNextLevel { get; }

        public PlayerGainedExperience(Guid aggregateId, int version, string playerName, int experienceGained, int totalExperience, int experienceToNextLevel)
            : base(aggregateId, version)
        {
            PlayerName = playerName;
            ExperienceGained = experienceGained;
            TotalExperience = totalExperience;
            ExperienceToNextLevel = experienceToNextLevel;
        }
    }
}
