using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;

namespace Portfolio.Domain.Services
{
    public class ContatoService : DomainService<Contato>, IContatoService
    {
        private readonly IContatoRepository _contatoRepository;

        public ContatoService(IContatoRepository repository) : base(repository)
        {
            _contatoRepository = repository;
        }

        public Task<Contato?> GetUnicoAsync(CancellationToken cancellationToken = default)
            => _contatoRepository.GetUnicoAsync(cancellationToken);
    }
}
