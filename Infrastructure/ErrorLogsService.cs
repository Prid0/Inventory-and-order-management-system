using Pim.Data.Infrastructure;
using Pim.Data.Models;
using Pim.Service.IService;

namespace Pim.Service
{
    public class ErrorLogsService : IErrorLogsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ErrorLogsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddAsync(ErrorLog log)
        {
            await _unitOfWork.ErrorLogRepository.Add(log);
            await _unitOfWork.Commit();
        }
    }
}
