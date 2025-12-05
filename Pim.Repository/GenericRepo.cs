using Microsoft.EntityFrameworkCore;
using Pim.Data;
using Pim.Repository.IRepository;
namespace Pim.IRepository
{

    public class GenericRepo<T> : IgenericRepo<T> where T : class
    {
        private readonly ApplicationDbContext _apiContext;
        private readonly DbSet<T> _dbSet;
        public GenericRepo(ApplicationDbContext apiContext)
        {
            _apiContext = apiContext;
            _dbSet = _apiContext.Set<T>();
        }

        public async Task Add(T obj)
        {
            var result = _dbSet.Add(obj);

        }

        public async Task Delete(T obj)
        {
            _dbSet.Remove(obj);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _dbSet.ToListAsync();
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

        public async Task SaveChanges()
        {
            await _apiContext.SaveChangesAsync();
        }

    }
}
