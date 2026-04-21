using Portfolio.Application.DTOs.Habilidades;
using Portfolio.Application.Interface;
using Portfolio.Domain.Entities;

namespace PortfolioApi.Controllers
{
    public sealed class HabilidadesController : BaseController<Habilidade, HabilidadeDto>
    {
        public HabilidadesController(IHabilidadesAppService service) : base(service)
        {
        }
    }
}
