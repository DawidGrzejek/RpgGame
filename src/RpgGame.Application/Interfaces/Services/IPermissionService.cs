using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RpgGame.Application.Interfaces.Services
{
    public interface IPermissionService
    {
        Task RefreshUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<bool> UserHasPermissionAsync(Guid userId, string permission, CancellationToken cancellationToken = default);
    }
}