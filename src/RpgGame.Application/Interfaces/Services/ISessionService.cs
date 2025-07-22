using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RpgGame.Application.Interfaces.Services
{
    public interface ISessionService
    {
        Task TerminateAllUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default);
        Task TerminateSessionAsync(string sessionId, CancellationToken cancellationToken = default);
    }
}