using Portfolio.Domain.Interface;

namespace Portfolio.Infrastructure.Resolvers
{
    public sealed class CurrentUserResolver : IUserResolver
    {
        public Guid GetCurrentUserId() => Guid.Empty;
    }
}
