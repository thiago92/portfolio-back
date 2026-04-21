using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;

namespace Portfolio.Domain.Services
{
    public class HabilidadeService : DomainService<Habilidade>, IHabilidadeService
    {
        public HabilidadeService(IHabilidadeRepository repository) : base(repository)
        {
        }
    }
}
