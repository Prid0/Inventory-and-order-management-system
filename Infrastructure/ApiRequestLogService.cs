using Pim.Data.Infrastructure;
using Pim.Data.Models;
using Pim.Service.IService;

namespace Pim.Service
{
    public class ApiRequestLogService : IApiRequestLogService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ApiRequestLogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddAsync(ApiRequestLog log)
        {
            await _unitOfWork.ApiRequestLogRepository.Add(log);
            await _unitOfWork.Commit();
        }

    }
}
