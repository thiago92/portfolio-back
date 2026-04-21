using Portfolio.Application.DTOs.Contatos;

namespace Portfolio.Application.Interface
{
    public interface IContatoAppService
    {
        Task<ContatoDto?> GetAsync(CancellationToken cancellationToken = default);
        Task<ContatoDto> AtualizarAsync(ContatoDto dto, CancellationToken cancellationToken = default);
    }
}
