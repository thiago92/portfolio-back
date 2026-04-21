using FluentValidation;
using MapsterMapper;
using Portfolio.Application.DTOs.Trabalhos;
using Portfolio.Application.Interface;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;

namespace Portfolio.Application.Services
{
    public class TrabalhosAppService : AppService<Trabalho, TrabalhoDto>, ITrabalhosAppService
    {
        public TrabalhosAppService(
            ITrabalhoService domainService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IEnumerable<IValidator<TrabalhoDto>> validators)
            : base(domainService, unitOfWork, mapper, validators)
        {
        }
    }
}
