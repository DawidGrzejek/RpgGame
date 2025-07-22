using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RpgGame.Domain.Entities.Users;
using RpgGame.Domain.Interfaces.Repositories;

namespace RpgGame.Application.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByAspNetUserIdAsync(string aspNetUserId);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> UsernameExistsAsync(string username);
        Task<List<User>> GetUsersByRoleAsync(string role);
    }
}