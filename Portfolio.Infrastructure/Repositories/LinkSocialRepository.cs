using Microsoft.Extensions.Caching.Memory;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Repositories
{
    public class LinkSocialRepository : GenericRepository<LinkSocial>, ILinkSocialRepository
    {
        public LinkSocialRepository(AppDbContext context, IMemoryCache cache) : base(context, cache)
        {
        }
    }
}
