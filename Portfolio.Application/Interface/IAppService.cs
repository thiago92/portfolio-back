using Portfolio.Domain.Entities;

namespace Portfolio.Application.Interface
{
    public interface IAppService<T> where T : EntityBase
    {
        IEnumerable<T> GetAll();
        T Get(Guid id);
        void Create(T entity);
        void Update(T entity);
        void Delete(Guid id);
    }
}
