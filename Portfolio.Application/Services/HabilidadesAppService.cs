using FluentValidation;
using MapsterMapper;
using Portfolio.Application.DTOs.Habilidades;
using Portfolio.Application.Interface;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;

namespace Portfolio.Application.Services
{
    public class HabilidadesAppService : AppService<Habilidade, HabilidadeDto>, IHabilidadesAppService
    {
        public HabilidadesAppService(
            IHabilidadeService domainService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IEnumerable<IValidator<HabilidadeDto>> validators)
            : base(domainService, unitOfWork, mapper, validators)
        {
        }
    }
}
