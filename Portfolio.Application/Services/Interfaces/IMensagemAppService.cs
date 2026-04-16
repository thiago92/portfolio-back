using PortfolioApi.Domain.Entities;

namespace Portfolio.Application.Services.Interfaces
{
    public interface IMensagemAppService
    {
        List<Mensagem> ObterTodas();
        Mensagem Criar(Mensagem mensagem);
    }
}
