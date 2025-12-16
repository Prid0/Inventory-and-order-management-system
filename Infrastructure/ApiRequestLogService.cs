using Pim.Data.Infrastructure;
using Pim.Data.Models;
using Pim.Service.IService;

namespace Pim.Service
{
    public class ApiRequestLogService : IApiRequestLogService
    {
        private readonly IUnitOfWork _uow;

        public ApiRequestLogService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task AddAsync(ApiRequestLog log)
        {
            await _uow.ApiRequestLogRepository.Add(log);
            await _uow.Commit();
        }

    }
}
