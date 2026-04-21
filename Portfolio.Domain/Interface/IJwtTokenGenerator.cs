using Portfolio.Domain.Entities;

namespace Portfolio.Domain.Interface
{
    public interface IJwtTokenGenerator
    {
        AccessTokenResult Generate(User user);
    }

    public sealed record AccessTokenResult(string Token, DateTime ExpiresAt);
}
