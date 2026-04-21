using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;

namespace Portfolio.Domain.Services
{
    public class UserService : DomainService<User>, IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository repository) : base(repository)
        {
            _userRepository = repository;
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
            => _userRepository.GetByEmailAsync(email, cancellationToken);
    }
}
