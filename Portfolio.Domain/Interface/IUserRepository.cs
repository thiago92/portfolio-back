using Portfolio.Domain.Entities;

namespace Portfolio.Domain.Interface
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    }
}
