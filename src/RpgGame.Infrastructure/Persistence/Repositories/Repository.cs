using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RpgGame.Domain.Base;
using RpgGame.Domain.Common;
using RpgGame.Domain.Entities.Configuration;
using RpgGame.Domain.Enums;
using RpgGame.Domain.Interfaces.Repositories;
using RpgGame.Infrastructure.Persistence.EFCore;

namespace RpgGame.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Generic repository base implementation using Entity Framework
    /// Provides common CRUD operations for all entities
    /// </summary>
    /// <typeparam name="T">Entity type that inherits from DomainEntity</typeparam>
    public abstract class Repository<T> : IRepository<T> where T : DomainEntity
    {
        protected readonly GameDbContext _context;
        protected readonly DbSet<T> _dbSet;

        protected Repository(GameDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<T>();
        }

        public virtual async Task<OperationResult<T>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                return entity != null
                    ? OperationResult<T>.Success(entity)
                    : OperationResult<T>.Failure(new OperationError("Entity not found.", $"No entity with ID {id} exists."));
            }
            catch (Exception ex)
            {
                return OperationResult<T>.Failure(new OperationError("An error occurred while retrieving the entity.", ex.Message));
            }
        }

        public virtual async Task<OperationResult<IEnumerable<T>>> GetAllAsync()
        {
            try
            {
                var entities = await _dbSet.ToListAsync();
                return OperationResult<IEnumerable<T>>.Success(entities);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<T>>.Failure(new OperationError("An error occurred while retrieving entities.", ex.Message));
            }
        }

        public virtual async Task<OperationResult> AddAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                await _dbSet.AddAsync(entity);
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.Failure(new OperationError("An error occurred while adding the entity.", ex.Message));
            }
        }

        public virtual async Task<OperationResult> UpdateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                _dbSet.Update(entity);
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.Failure(new OperationError("An error occurred while updating the entity.", ex.Message));
            }
        }

        public virtual async Task<OperationResult> DeleteAsync(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            try
            {
                _dbSet.Remove(entity);
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.Failure(new OperationError("An error occurred while deleting the entity.", ex.Message));
            }
        }

        public virtual async Task<OperationResult> SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.Failure(new OperationError("An error occurred while saving changes.", ex.Message));
            }
        }

        public virtual async Task<OperationResult<bool>> ExistsAsync(Guid id)
        {
            try
            {
                var exists = await _dbSet.AnyAsync(e => e.Id == id);
                return OperationResult<bool>.Success(exists);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Failure(new OperationError("An error occurred while checking existence.", ex.Message));
            }
        }
    }
}