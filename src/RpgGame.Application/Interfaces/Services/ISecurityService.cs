using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RpgGame.Application.Interfaces.Services
{
    public interface ISecurityService
    {
        Task MonitorLoginAsync(Guid userId, string? ipAddress, string? userAgent, DateTime loginTime, CancellationToken cancellationToken = default);
        Task CheckSuspiciousActivityAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}