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

        public virtual async Task<T> Get(Expression<Func<T, bool>> filter, bool track = true, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            if (track)
            {
                return await query.FirstOrDefaultAsync();
            }
            else
            {
                return await query.AsNoTracking().FirstOrDefaultAsync();
            }
        }

        public async Task<IEnumerable<T>> GetMultiple(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes)
        {
            var list = dbSet.Where(filter);
            foreach (var include in includes)
            {
                list = list.Include(include);
            }
            return await list.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAll(params Expression<Func<T, object>>[] includes)
        {
            var query = dbSet.AsQueryable();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.AsNoTracking().ToListAsync();


        }

        public virtual bool Remove(int id)
        {
            throw new NotImplementedException();
        }

        public virtual bool Remove(string id)
        {
            throw new NotImplementedException();
        }
        public bool RemoveRange(IEnumerable<T> entity)
        {
            throw new NotImplementedException();
        }

        public bool Update(T entity)
        {
            dbSet.Update(entity);
            return true;
        }

        public async Task AddRange(List<T> entity)
        {
            await dbSet.AddRangeAsync(entity);
        }

        public bool Any()
        {
            return dbSet.Any();
        }

        public bool Remove(T entity)
        {
            dbSet.Remove(entity);
            return true;
        }
    }
}