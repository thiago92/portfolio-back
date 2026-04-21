using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;

namespace Portfolio.Domain.Services
{
    public class ProjetoCarouselService : DomainService<ProjetoCarousel>, IProjetoCarouselService
    {
        public ProjetoCarouselService(IProjetoCarouselRepository repository) : base(repository)
        {
        }
    }
}
