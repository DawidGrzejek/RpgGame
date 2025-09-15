using MediatR;

namespace RpgGame.Application.Queries.UserManagement
{
    public class UserManagementDto
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }
        public List<string> Roles { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }
    }

    public record GetAllUsersQuery(
        int Page = 1,
        int PageSize = 20,
        string? SearchTerm = null,
        string? RoleFilter = null
    ) : IRequest<PagedResult<UserManagementDto>>;

    public record GetUserByIdQuery(string Id) : IRequest<UserManagementDto?>;

    public record GetUserByUsernameQuery(string Username) : IRequest<UserManagementDto?>;

    public record GetUserByEmailQuery(string Email) : IRequest<UserManagementDto?>;

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;
    }
}