using Portfolio.Application.Interface;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Interface;
namespace Portfolio.Application.Services
{
    public class AppService<T> : IAppService<T> where T : EntityBase
    {
        protected readonly IRepository<T> _repository;

        public AppService(IRepository<T> repository)
        {
            _repository = repository;
        }

        public IEnumerable<T> GetAll() => _repository.GetAll();

        public T Get(Guid id) => _repository.GetById(id);

        public void Create(T entity) => _repository.Add(entity);

        public void Update(T entity) => _repository.Update(entity);

        public void Delete(Guid id) => _repository.Delete(id);
    }
}
