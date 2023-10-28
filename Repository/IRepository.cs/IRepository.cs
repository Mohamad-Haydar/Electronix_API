using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;

namespace Web_API.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        public Task<IEnumerable<T>> GetAll();
        Task<T> Get(Expression<Func<T, bool>> filter);
        Task<bool> Add(T entity);
        Task<bool> Update(T entity);
        Task<bool> Remove(int id);
        Task<bool> RemoveRange(IEnumerable<T> entity);
    }
}