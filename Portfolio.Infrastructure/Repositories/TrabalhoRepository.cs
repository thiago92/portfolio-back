using Microsoft.Extensions.Caching.Memory;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories
{
    public class TrabalhoRepository : GenericRepository<Trabalho>, ITrabalhoRepository
    {
        public TrabalhoRepository(AppDbContext context, IMemoryCache cache) : base(context, cache)
        {
        }
    }
}
