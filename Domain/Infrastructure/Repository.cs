using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Pim.Data.Infrastructure
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _apiContext;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext apiContext)
        {
            _apiContext = apiContext;
            _dbSet = _apiContext.Set<T>();
        }

        public async Task Add(T obj)
        {
            _dbSet.Add(obj);
        }

        public async Task Delete(T obj)
        {
            _dbSet.Remove(obj);
        }

        public async Task<List<T>> GetAll(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null
                ? await _dbSet.AsNoTracking().ToListAsync()
                : await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
        }

        public async Task<T> GetById(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task Update(T obj)
        {
            _dbSet.Attach(obj);
            _apiContext.Entry(obj).State = EntityState.Modified;
        }

    }
}
