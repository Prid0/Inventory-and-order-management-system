namespace Pim.Data.Infrastructure
{
    public interface IRepository<T> where T : class
    {
        Task Add(T obj);
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(int id);
        Task Update(T obj);
        Task Delete(T obj);
        //Task ConstructSQLCommand(string commandText, params object[] parameters);
        //Task<List<T>> ExecuteStoredProcedureListAsync<TResult>(string commandText, params object[] parameters);

    }
}
