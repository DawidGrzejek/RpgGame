using MediatR;

namespace RpgGame.Application.Commands
{
    // Marker interface to identify commands for behaviors
    public interface ICommand : IRequest { }

    // Generic version for commands that return a result
    public interface ICommand<TResult> : IRequest<TResult>, ICommand { }
}