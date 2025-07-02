using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RpgGame.Domain.Base;

namespace RpgGame.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Generic repository interface for basic CRUD operations
    /// </summary>
    /// <typeparam name="T">Entity type that inherits from DomainEntity</typeparam>
    public interface IRepository<T> where T : DomainEntity
    {
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task SaveChangesAsync();
        Task<bool> ExistsAsync(Guid id);
    }
}