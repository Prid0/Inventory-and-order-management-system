using Microsoft.EntityFrameworkCore.Storage;
using Pim.Data.Repository.ApiRequestLogs;
using Pim.Data.Repository.Category;
using Pim.Data.Repository.ErrorLogs;
using Pim.Data.Repository.Orders;
using Pim.Data.Repository.Products;
using Pim.Data.Repository.Role;
using Pim.Data.Repository.User;

namespace Pim.Data.Infrastructure
{
    public interface IUnitOfWork
    {
        public IUserRepository UserRepository { get; }

        public IProductRepository ProductRepository { get; }

        public IRoleRepository RoleRepository { get; }

        public IOrderRepository OrderRepository { get; }

        public ICategoryRepository CategoryRepository { get; }
        public IErrorLogRepository ErrorLogRepository { get; }
        public IApiRequestLogRepository ApiRequestLogRepository { get; }
        Task<IDbContextTransaction> BeginTransactionAsync();

        Task<int> Commit();

    }
}


