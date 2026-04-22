using Microsoft.Extensions.Caching.Memory;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories
{
    public class ContatoRepository : GenericRepository<Contato>, IContatoRepository
    {
        public ContatoRepository(AppDbContext context, IMemoryCache cache) : base(context, cache)
        {
        }

        public async Task<Contato?> GetUnicoAsync(CancellationToken cancellationToken = default)
        {
            var all = await GetAllAsync(cancellationToken);
            return all.FirstOrDefault();
        }
    }
}
