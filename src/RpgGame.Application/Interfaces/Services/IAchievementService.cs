using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RpgGame.Application.Interfaces.Services
{
    public interface IAchievementService
    {
        Task CheckForBonusAchievementsAsync(Guid userId, string unlockedAchievement, CancellationToken cancellationToken = default);
        Task CheckForNewAchievementsAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}