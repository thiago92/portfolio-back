using FluentValidation;
using MapsterMapper;
using Portfolio.Application.DTOs.Mensagens;
using Portfolio.Application.Interface;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;

namespace Portfolio.Application.Services
{
    public class MensagensAppService : AppService<Mensagem, MensagemDto>, IMensagensAppService
    {
        public MensagensAppService(
            IMensagemService domainService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IEnumerable<IValidator<MensagemDto>> validators)
            : base(domainService, unitOfWork, mapper, validators)
        {
        }
    }
}
