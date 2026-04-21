using Portfolio.Domain.Entities;

namespace Portfolio.Domain.Interface
{
    public interface IContatoRepository : IRepository<Contato>
    {
        Task<Contato?> GetUnicoAsync(CancellationToken cancellationToken = default);
    }
}
