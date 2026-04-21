using Portfolio.Application.DTOs.LinksSociais;
using Portfolio.Application.Interface;
using Portfolio.Domain.Entities;

namespace PortfolioApi.Controllers
{
    public sealed class LinksSociaisController : BaseController<LinkSocial, LinkSocialDto>
    {
        public LinksSociaisController(ILinksSociaisAppService service) : base(service)
        {
        }
    }
}
