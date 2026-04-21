using Portfolio.Application.DTOs.Trabalhos;
using Portfolio.Application.Interface;
using Portfolio.Domain.Entities;

namespace PortfolioApi.Controllers
{
    public sealed class TrabalhosController : BaseController<Trabalho, TrabalhoDto>
    {
        public TrabalhosController(ITrabalhosAppService service) : base(service)
        {
        }
    }
}
