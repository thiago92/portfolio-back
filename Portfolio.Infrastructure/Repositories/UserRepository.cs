using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
            => _dbSet.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }
}
