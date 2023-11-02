using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Web_API.Data;
using Web_API.Repository.IRepository;

namespace Web_API.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger _logger;
        protected DbSet<T> dbSet;

        public Repository(ApplicationDbContext db, ILogger logger)
        {
            _db = db;
            _logger = logger;
            dbSet = _db.Set<T>();
        }
        public virtual async Task<bool> Add(T entity)
        {
            await dbSet.AddAsync(entity);
            return true;
        }

        public virtual async Task<T> Get(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetMultiple(Expression<Func<T, bool>> filter)
        {
            var list = await dbSet.Where(filter).ToListAsync();
            return list;
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await dbSet.ToListAsync();
        }

        public virtual async Task<bool> Remove(int id)
        {
            throw new NotImplementedException();
        }

        public virtual Task<bool> RemoveRange(IEnumerable<T> entity)
        {
            throw new NotImplementedException();
        }

        public virtual Task<bool> Update(T entity)
        {
            throw new NotImplementedException();
        }

        public async Task AddRange(List<T> entity)
        {
            await dbSet.AddRangeAsync(entity);
        }

        public bool Any()
        {
            return dbSet.Any();
        }
    }
}