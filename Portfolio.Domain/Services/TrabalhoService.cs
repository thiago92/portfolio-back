using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;

namespace Portfolio.Domain.Services
{
    public class TrabalhoService : DomainService<Trabalho>, ITrabalhoService
    {
        public TrabalhoService(ITrabalhoRepository repository) : base(repository)
        {
        }
    }
}
