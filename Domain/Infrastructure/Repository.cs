using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

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

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
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

        public string ConstructSQLCommand(string commandText, params object[] parameters)
        {
            for (int i = 0; i <= parameters.Length - 1; i++)
            {
                var p = parameters[i] as DbParameter;
                if (p == null)
                    throw new Exception("Not support parameter type");

                if (p.DbType == DbType.AnsiString || p.DbType == DbType.Int32 ||
                    p.DbType == DbType.Date || p.DbType == DbType.Decimal)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(p.Value)))
                        p.Value = DBNull.Value;
                }

                commandText += i == 0 ? " " : ", ";
                commandText += "@" + p.ParameterName;

                if (p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output)
                {
                    commandText += " output";

                    if (p.DbType == DbType.AnsiString)
                    {
                        p.Size = 1000;
                    }
                }
            }

            return commandText;
        }

        public async Task<List<T>> ExecuteStoredProcedureListAsync<T>(string commandText, params object[] parameters)
            where T : class
        {
            if (parameters != null && parameters.Length > 0)
            {
                commandText = ConstructSQLCommand(commandText, parameters);
            }

            var result = await _apiContext.Set<T>().FromSqlRaw(commandText, parameters).ToListAsync();

            return result;
        }
    }
}
