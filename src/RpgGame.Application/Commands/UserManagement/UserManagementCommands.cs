using MediatR;
using RpgGame.Domain.Common;

namespace RpgGame.Application.Commands.UserManagement
{
    public record CreateUserCommand(
        string Username,
        string Email,
        string Password,
        List<string> Roles,
        bool EmailConfirmed = false,
        bool LockoutEnabled = true
    ) : IRequest<OperationResult>;

    public record UpdateUserCommand(
        string Id,
        string Username,
        string Email,
        bool EmailConfirmed,
        bool LockoutEnabled,
        DateTimeOffset? LockoutEnd,
        List<string> Roles,
        bool IsActive
    ) : IRequest<OperationResult>;

    public record DeleteUserCommand(string Id) : IRequest<OperationResult>;

    public record ChangeUserPasswordCommand(
        string UserId,
        string NewPassword
    ) : IRequest<OperationResult>;

    public record LockUserCommand(
        string UserId,
        DateTimeOffset? LockoutEnd,
        string Reason
    ) : IRequest<OperationResult>;

    public record UnlockUserCommand(string UserId) : IRequest<OperationResult>;

    public record AssignRoleToUserCommand(
        string UserId,
        string RoleName
    ) : IRequest<OperationResult>;

    public record RemoveRoleFromUserCommand(
        string UserId,
        string RoleName
    ) : IRequest<OperationResult>;
}