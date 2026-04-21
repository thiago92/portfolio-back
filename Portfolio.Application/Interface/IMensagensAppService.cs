using Portfolio.Application.DTOs.Mensagens;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Interface
{
    public interface IMensagensAppService : IAppService<Mensagem, MensagemDto>
    {
    }
}
