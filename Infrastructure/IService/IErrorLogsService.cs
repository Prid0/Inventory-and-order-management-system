using Pim.Data.Models;

namespace Pim.Service.IService
{
    public interface IErrorLogsService
    {
        Task AddAsync(ErrorLog log);
    }
}
