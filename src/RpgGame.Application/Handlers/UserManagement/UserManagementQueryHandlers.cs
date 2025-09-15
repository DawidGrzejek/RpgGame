using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RpgGame.Application.Interfaces.Repositories;
using RpgGame.Application.Queries.UserManagement;

namespace RpgGame.Application.Handlers.UserManagement
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PagedResult<UserManagementDto>>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetAllUsersQueryHandler> _logger;

        public GetAllUsersQueryHandler(
            UserManager<IdentityUser> userManager,
            IUserRepository userRepository,
            ILogger<GetAllUsersQueryHandler> logger)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<PagedResult<UserManagementDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _userManager.Users.AsQueryable();

                // Apply search filter
                if (!string.IsNullOrEmpty(request.SearchTerm))
                {
                    query = query.Where(u => u.UserName!.Contains(request.SearchTerm) || 
                                           u.Email!.Contains(request.SearchTerm));
                }

                // Apply role filter
                if (!string.IsNullOrEmpty(request.RoleFilter))
                {
                    var usersInRole = await _userManager.GetUsersInRoleAsync(request.RoleFilter);
                    var userIds = usersInRole.Select(u => u.Id).ToList();
                    query = query.Where(u => userIds.Contains(u.Id));
                }

                var totalCount = await query.CountAsync(cancellationToken);
                
                var users = await query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken);

                var userDtos = new List<UserManagementDto>();

                foreach (var user in users)
                {
                    var domainUser = await _userRepository.GetByAspNetUserIdAsync(user.Id);
                    var roles = await _userManager.GetRolesAsync(user);

                    userDtos.Add(new UserManagementDto
                    {
                        Id = user.Id,
                        Username = user.UserName ?? string.Empty,
                        Email = user.Email ?? string.Empty,
                        EmailConfirmed = user.EmailConfirmed,
                        LockoutEnabled = user.LockoutEnabled,
                        LockoutEnd = user.LockoutEnd,
                        AccessFailedCount = user.AccessFailedCount,
                        Roles = roles.ToList(),
                        CreatedAt = domainUser?.CreatedAt ?? DateTime.MinValue,
                        LastLoginAt = domainUser?.LastLoginAt,
                        IsActive = domainUser?.IsActive ?? true
                    });
                }

                return new PagedResult<UserManagementDto>
                {
                    Items = userDtos,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                return new PagedResult<UserManagementDto>
                {
                    Items = new List<UserManagementDto>(),
                    TotalCount = 0,
                    Page = request.Page,
                    PageSize = request.PageSize
                };
            }
        }
    }

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserManagementDto?>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetUserByIdQueryHandler> _logger;

        public GetUserByIdQueryHandler(
            UserManager<IdentityUser> userManager,
            IUserRepository userRepository,
            ILogger<GetUserByIdQueryHandler> logger)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UserManagementDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.Id);
                if (user == null) return null;

                var domainUser = await _userRepository.GetByAspNetUserIdAsync(user.Id);
                var roles = await _userManager.GetRolesAsync(user);

                return new UserManagementDto
                {
                    Id = user.Id,
                    Username = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEnd = user.LockoutEnd,
                    AccessFailedCount = user.AccessFailedCount,
                    Roles = roles.ToList(),
                    CreatedAt = domainUser?.CreatedAt ?? DateTime.MinValue,
                    LastLoginAt = domainUser?.LastLoginAt,
                    IsActive = domainUser?.IsActive ?? true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user {UserId}", request.Id);
                return null;
            }
        }
    }

    public class GetUserByUsernameQueryHandler : IRequestHandler<GetUserByUsernameQuery, UserManagementDto?>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetUserByUsernameQueryHandler> _logger;

        public GetUserByUsernameQueryHandler(
            UserManager<IdentityUser> userManager,
            IUserRepository userRepository,
            ILogger<GetUserByUsernameQueryHandler> logger)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UserManagementDto?> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(request.Username);
                if (user == null) return null;

                var domainUser = await _userRepository.GetByAspNetUserIdAsync(user.Id);
                var roles = await _userManager.GetRolesAsync(user);

                return new UserManagementDto
                {
                    Id = user.Id,
                    Username = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEnd = user.LockoutEnd,
                    AccessFailedCount = user.AccessFailedCount,
                    Roles = roles.ToList(),
                    CreatedAt = domainUser?.CreatedAt ?? DateTime.MinValue,
                    LastLoginAt = domainUser?.LastLoginAt,
                    IsActive = domainUser?.IsActive ?? true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by username {Username}", request.Username);
                return null;
            }
        }
    }

    public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, UserManagementDto?>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetUserByEmailQueryHandler> _logger;

        public GetUserByEmailQueryHandler(
            UserManager<IdentityUser> userManager,
            IUserRepository userRepository,
            ILogger<GetUserByEmailQueryHandler> logger)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UserManagementDto?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null) return null;

                var domainUser = await _userRepository.GetByAspNetUserIdAsync(user.Id);
                var roles = await _userManager.GetRolesAsync(user);

                return new UserManagementDto
                {
                    Id = user.Id,
                    Username = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEnd = user.LockoutEnd,
                    AccessFailedCount = user.AccessFailedCount,
                    Roles = roles.ToList(),
                    CreatedAt = domainUser?.CreatedAt ?? DateTime.MinValue,
                    LastLoginAt = domainUser?.LastLoginAt,
                    IsActive = domainUser?.IsActive ?? true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by email {Email}", request.Email);
                return null;
            }
        }
    }
}