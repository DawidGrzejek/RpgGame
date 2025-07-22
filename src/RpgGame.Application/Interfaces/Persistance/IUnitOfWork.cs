using System.Threading;
using System.Threading.Tasks;

namespace RpgGame.Application.Interfaces.Persistence
{
    /// <summary>
    /// Unit of Work pattern interface for managing transactions
    /// Defined in Application layer, implemented in Infrastructure layer
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Commits all pending changes to the database
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if changes were committed successfully</returns>
        Task<bool> CommitAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves changes to the database without returning success indicator
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Number of records affected</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Begins a new transaction
        /// </summary>
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits the current transaction
        /// </summary>
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Rolls back the current transaction
        /// </summary>
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}