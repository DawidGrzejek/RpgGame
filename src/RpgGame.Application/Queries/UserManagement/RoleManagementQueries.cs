using MediatR;

namespace RpgGame.Application.Queries.UserManagement
{
    public class RoleManagementDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string NormalizedName { get; set; } = string.Empty;
        public int UserCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public record GetAllRolesQuery() : IRequest<List<RoleManagementDto>>;

    public record GetRoleByIdQuery(string Id) : IRequest<RoleManagementDto?>;

    public record GetRoleByNameQuery(string Name) : IRequest<RoleManagementDto?>;
}