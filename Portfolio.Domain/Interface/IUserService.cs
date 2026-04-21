using Portfolio.Domain.Entities;

namespace Portfolio.Domain.Interface
{
    public interface IUserService : IDomainService<User>
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    }
}
