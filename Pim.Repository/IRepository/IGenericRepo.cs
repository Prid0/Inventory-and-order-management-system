namespace Pim.Repository.IRepository
{
    public interface IgenericRepo<T> where T : class
    {
        Task Add(T obj);
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(int id);
        Task Update(T obj);
        Task Delete(T obj);
        Task SaveChanges();
    }
}
