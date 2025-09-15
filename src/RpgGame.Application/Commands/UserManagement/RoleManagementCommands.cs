using MediatR;
using RpgGame.Domain.Common;

namespace RpgGame.Application.Commands.UserManagement
{
    public record CreateRoleCommand(string Name) : IRequest<OperationResult>;

    public record UpdateRoleCommand(string Id, string Name) : IRequest<OperationResult>;

    public record DeleteRoleCommand(string Id) : IRequest<OperationResult>;
}