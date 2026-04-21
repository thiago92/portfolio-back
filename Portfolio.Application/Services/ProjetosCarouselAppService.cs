using FluentValidation;
using MapsterMapper;
using Portfolio.Application.DTOs.ProjetosCarousel;
using Portfolio.Application.Interface;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;

namespace Portfolio.Application.Services
{
    public class ProjetosCarouselAppService : AppService<ProjetoCarousel, ProjetoCarouselDto>, IProjetosCarouselAppService
    {
        public ProjetosCarouselAppService(
            IProjetoCarouselService domainService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IEnumerable<IValidator<ProjetoCarouselDto>> validators)
            : base(domainService, unitOfWork, mapper, validators)
        {
        }
    }
}
