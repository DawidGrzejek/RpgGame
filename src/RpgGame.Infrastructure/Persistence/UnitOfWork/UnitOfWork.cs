using Microsoft.EntityFrameworkCore.Storage;
using RpgGame.Application.Events;
using RpgGame.Application.Interfaces.Persistence;
using RpgGame.Domain.Base;
using RpgGame.Infrastructure.Persistence.EFCore;

namespace RpgGame.Infrastructure.Persistence.UnitOfWork
{
    /// <summary>
    /// Implementation of Unit of Work pattern using Entity Framework
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GameDbContext _context;
        private readonly IEventDispatcher _eventDispatcher;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(GameDbContext context, IEventDispatcher eventDispatcher)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
        }

        public async Task<bool> CommitAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // Collect domain events before saving
                var entitiesWithEvents = _context.ChangeTracker.Entries<DomainEntity>()
                    .Where(e => e.Entity.DomainEvents.Any())
                    .Select(e => e.Entity)
                    .ToList();

                var domainEvents = entitiesWithEvents
                    .SelectMany(e => e.DomainEvents)
                    .ToList();

                // Save changes first
                var result = await _context.SaveChangesAsync(cancellationToken);

                // Then dispatch events
                if (domainEvents.Any())
                {
                    await _eventDispatcher.DispatchAsync(domainEvents, cancellationToken);
                    
                    // Clear events after successful dispatch
                    foreach (var entity in entitiesWithEvents)
                    {
                        entity.ClearDomainEvents();
                    }
                }

                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("A transaction is already in progress.");
            }

            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction in progress.");
            }

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                await _transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction in progress.");
            }

            try
            {
                await _transaction.RollbackAsync(cancellationToken);
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
        }
    }
}
