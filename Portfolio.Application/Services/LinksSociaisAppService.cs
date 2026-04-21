using FluentValidation;
using MapsterMapper;
using Portfolio.Application.DTOs.LinksSociais;
using Portfolio.Application.Interface;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;

namespace Portfolio.Application.Services
{
    public class LinksSociaisAppService : AppService<LinkSocial, LinkSocialDto>, ILinksSociaisAppService
    {
        public LinksSociaisAppService(
            ILinkSocialService domainService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IEnumerable<IValidator<LinkSocialDto>> validators)
            : base(domainService, unitOfWork, mapper, validators)
        {
        }
    }
}
