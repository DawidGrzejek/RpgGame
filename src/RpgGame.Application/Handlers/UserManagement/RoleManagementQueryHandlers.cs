using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using RpgGame.Application.Queries.UserManagement;
using Microsoft.EntityFrameworkCore;

namespace RpgGame.Application.Handlers.UserManagement
{
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, List<RoleManagementDto>>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<GetAllRolesQueryHandler> _logger;

        public GetAllRolesQueryHandler(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            ILogger<GetAllRolesQueryHandler> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<List<RoleManagementDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var roles = await _roleManager.Roles.ToListAsync(cancellationToken);
                var roleDtos = new List<RoleManagementDto>();

                foreach (var role in roles)
                {
                    var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
                    
                    roleDtos.Add(new RoleManagementDto
                    {
                        Id = role.Id,
                        Name = role.Name ?? string.Empty,
                        NormalizedName = role.NormalizedName ?? string.Empty,
                        UserCount = usersInRole.Count,
                        CreatedAt = DateTime.UtcNow // Identity roles don't have CreatedAt, using current time
                    });
                }

                return roleDtos.OrderBy(r => r.Name).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving roles");
                return new List<RoleManagementDto>();
            }
        }
    }

    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RoleManagementDto?>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<GetRoleByIdQueryHandler> _logger;

        public GetRoleByIdQueryHandler(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            ILogger<GetRoleByIdQueryHandler> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<RoleManagementDto?> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(request.Id);
                if (role == null) return null;

                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);

                return new RoleManagementDto
                {
                    Id = role.Id,
                    Name = role.Name ?? string.Empty,
                    NormalizedName = role.NormalizedName ?? string.Empty,
                    UserCount = usersInRole.Count,
                    CreatedAt = DateTime.UtcNow // Identity roles don't have CreatedAt, using current time
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving role {RoleId}", request.Id);
                return null;
            }
        }
    }

    public class GetRoleByNameQueryHandler : IRequestHandler<GetRoleByNameQuery, RoleManagementDto?>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<GetRoleByNameQueryHandler> _logger;

        public GetRoleByNameQueryHandler(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            ILogger<GetRoleByNameQueryHandler> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<RoleManagementDto?> Handle(GetRoleByNameQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var role = await _roleManager.FindByNameAsync(request.Name);
                if (role == null) return null;

                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);

                return new RoleManagementDto
                {
                    Id = role.Id,
                    Name = role.Name ?? string.Empty,
                    NormalizedName = role.NormalizedName ?? string.Empty,
                    UserCount = usersInRole.Count,
                    CreatedAt = DateTime.UtcNow // Identity roles don't have CreatedAt, using current time
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving role by name {RoleName}", request.Name);
                return null;
            }
        }
    }
}