using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories
{
    public class HabilidadeRepository : GenericRepository<Habilidade>, IHabilidadeRepository
    {
        public HabilidadeRepository(AppDbContext context) : base(context)
        {
        }
    }
}
