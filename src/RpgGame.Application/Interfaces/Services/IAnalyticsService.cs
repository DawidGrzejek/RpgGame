using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RpgGame.Application.Interfaces.Services
{
    public interface IAnalyticsService
    {
        Task TrackUserLoginAsync(Guid userId, string username, DateTime loginTime, string? ipAddress, CancellationToken cancellationToken = default);
        Task TrackUserActionAsync(Guid userId, string action, Dictionary<string, object>? properties = null, CancellationToken cancellationToken = default);
    }
}