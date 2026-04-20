using Portfolio.Application.Interface;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;

namespace Portfolio.Application.Services
{
    public class AppService<T> : IAppService<T> where T : Entity
    {
        protected readonly IRepository<T> _repository;

        public AppService(IRepository<T> repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
            => _repository.GetAllAsync(cancellationToken);

        public Task<T?> GetAsync(Guid id, CancellationToken cancellationToken = default)
            => _repository.GetByIdAsync(id, cancellationToken);

        public Task CreateAsync(T entity, CancellationToken cancellationToken = default)
            => _repository.AddAsync(entity, cancellationToken);

        public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
            => _repository.UpdateAsync(entity, cancellationToken);

        public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
            => _repository.DeleteAsync(id, cancellationToken);
    }
}
