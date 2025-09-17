using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RpgGame.Domain.Base;
using RpgGame.Domain.Common;

namespace RpgGame.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Generic repository interface for basic CRUD operations
    /// </summary>
    /// <typeparam name="T">Entity type that inherits from DomainEntity</typeparam>
    public interface IRepository<T> where T : DomainEntity
    {
        Task<OperationResult<T>> GetByIdAsync(Guid id);
        Task<OperationResult<IEnumerable<T>>> GetAllAsync();
        Task<OperationResult> AddAsync(T entity);
        Task<OperationResult> UpdateAsync(T entity);
        Task<OperationResult> DeleteAsync(T entity);
        Task<OperationResult> SaveChangesAsync();
        Task<OperationResult<bool>> ExistsAsync(Guid id);
    }
}