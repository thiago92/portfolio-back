using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories
{
    public class ProjetoCarouselRepository : GenericRepository<ProjetoCarousel>, IProjetoCarouselRepository
    {
        public ProjetoCarouselRepository(AppDbContext context) : base(context)
        {
        }
    }
}
