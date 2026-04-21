using Portfolio.Application.DTOs.ProjetosCarousel;
using Portfolio.Application.Interface;
using Portfolio.Domain.Entities;

namespace PortfolioApi.Controllers
{
    public sealed class ProjetosCarouselController : BaseController<ProjetoCarousel, ProjetoCarouselDto>
    {
        public ProjetosCarouselController(IProjetosCarouselAppService service) : base(service)
        {
        }
    }
}
