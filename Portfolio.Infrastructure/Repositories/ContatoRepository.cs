using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories
{
    public class ContatoRepository : GenericRepository<Contato>, IContatoRepository
    {
        public ContatoRepository(AppDbContext context) : base(context)
        {
        }

        public Task<Contato?> GetUnicoAsync(CancellationToken cancellationToken = default)
            => _dbSet.FirstOrDefaultAsync(cancellationToken);
    }
}
