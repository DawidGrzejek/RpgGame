using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RpgGame.Application.Interfaces.Repositories;
using RpgGame.Domain.Entities.Users;
using RpgGame.Infrastructure.Persistence.EFCore;

namespace RpgGame.Infrastructure.Persistence.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(GameDbContext context) : base(context) { }

        public async Task<User?> GetByAspNetUserIdAsync(string aspNetUserId)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.AspNetUserId == aspNetUserId);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _dbSet.AnyAsync(u => u.Username == username);
        }

        public async Task<List<User>> GetUsersByRoleAsync(string role)
        {
            return await _dbSet.Where(u => u.Roles.Contains(role)).ToListAsync();
        }
    }
}