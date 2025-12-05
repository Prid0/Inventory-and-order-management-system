using Pim.Data.Infrastructure;
using Pim.Data.Models;

namespace Pim.Service
{
    public class ErrorLogsService
    {
        private readonly IUnitOfWork _uow;

        public ErrorLogsService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task AddAsync(ErrorLog log)
        {
            await _uow.ErrorLogRepository.Add(log);
            await _uow.Commit();
        }
    }
}
