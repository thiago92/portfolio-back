using Portfolio.Domain.Entities;

namespace Portfolio.Domain.Interface
{
    public interface IContatoService : IDomainService<Contato>
    {
        Task<Contato?> GetUnicoAsync(CancellationToken cancellationToken = default);
    }
}
