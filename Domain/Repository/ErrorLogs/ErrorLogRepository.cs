using Pim.Data.Infrastructure;
using Pim.Data.Models;

namespace Pim.Data.Repository.ErrorLogs
{
    public class ErrorLogRepository : Repository<ErrorLog>, IErrorLogRepository
    {

        public ErrorLogRepository(ApplicationDbContext context) : base(context)
        {

        }

    }
}
