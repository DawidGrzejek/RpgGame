using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RpgGame.Infrastructure.Persistence.EFCore;

namespace RpgGame.Infrastructure.Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GameDbContext _context;

        public UnitOfWork(GameDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CommitAsync(CancellationToken cancellationToken = default)
        {
            // Return true if records were saved successfully
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
