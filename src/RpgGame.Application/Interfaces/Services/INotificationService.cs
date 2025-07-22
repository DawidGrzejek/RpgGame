using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RpgGame.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task SendNotificationAsync(Guid userId, string title, string message, CancellationToken cancellationToken = default);
        Task SendBroadcastNotificationAsync(string title, string message, CancellationToken cancellationToken = default);
    }
}