using RpgGame.Domain.Events.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpgGame.Application.Events.Handlers
{/// <summary>
 /// Handles player experience events for analytics
 /// </summary>
    public class PlayerExperienceHandler : IEventHandler<PlayerGainedExperience>
    {
        public async Task HandleAsync(PlayerGainedExperience domainEvent)
        {
            // Log experience gained for analytics
            Console.WriteLine($"Player {domainEvent.PlayerName} gained {domainEvent.ExperienceGained} experience");
            Console.WriteLine($"Total experience: {domainEvent.TotalExperience}/{domainEvent.ExperienceToNextLevel}");

            // Here you might:
            // 1. Update analytics database
            // 2. Check achievements
            // 3. Update UI notifications
        }
    }
}
