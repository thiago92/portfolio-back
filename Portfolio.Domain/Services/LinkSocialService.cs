using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;

namespace Portfolio.Domain.Services
{
    public class LinkSocialService : DomainService<LinkSocial>, ILinkSocialService
    {
        public LinkSocialService(ILinkSocialRepository repository) : base(repository)
        {
        }
    }
}
