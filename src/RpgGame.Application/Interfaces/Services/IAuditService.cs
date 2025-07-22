using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RpgGame.Application.Interfaces.Services
{
    public interface IAuditService
    {
        Task LogUserActionAsync(string performedBy, string action, string description, object? additionalData = null, CancellationToken cancellationToken = default);
    }
}