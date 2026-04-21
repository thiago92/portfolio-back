using Portfolio.Application.DTOs.Mensagens;
using Portfolio.Application.Interface;
using Portfolio.Domain.Entities;

namespace PortfolioApi.Controllers
{
    public sealed class MensagensController : BaseController<Mensagem, MensagemDto>
    {
        public MensagensController(IMensagensAppService service) : base(service)
        {
        }
    }
}
