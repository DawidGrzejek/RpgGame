using MediatR;

namespace RpgGame.Application.Queries
{
    // Marker interface to identify queries for behaviors
    public interface IQuery<TResult> : IRequest<TResult> { }
}