using Pim.Data.Models;

namespace Pim.Service.IService
{
    public interface IApiRequestLogService
    {
        Task AddAsync(ApiRequestLog log);
    }
}
