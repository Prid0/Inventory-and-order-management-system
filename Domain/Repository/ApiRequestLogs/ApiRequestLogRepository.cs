using Pim.Data.Infrastructure;
using Pim.Data.Models;

namespace Pim.Data.Repository.ApiRequestLogs
{
    public class ApiRequestLogRepository : Repository<ApiRequestLog>, IApiRequestLogRepository
    {
        public ApiRequestLogRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
