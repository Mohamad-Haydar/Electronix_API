using System.Linq.Expressions;

namespace Web_API.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        public Task<IEnumerable<T>> GetAll(params Expression<Func<T, object>>[] includes);
        Task<T> Get(Expression<Func<T, bool>> filter, bool track = true, params Expression<Func<T, object>>[] includes);
        public Task<IEnumerable<T>> GetMultiple(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes);

        Task<bool> Add(T entity);
        Task AddRange(List<T> entity);
        bool Any();
        bool Update(T entity);
        bool Remove(T entity);
        bool Remove(int id);
        bool Remove(string id);
        bool RemoveRange(IEnumerable<T> entity);
    }
}