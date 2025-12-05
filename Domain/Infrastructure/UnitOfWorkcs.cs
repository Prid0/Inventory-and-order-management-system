using Microsoft.EntityFrameworkCore.Storage;
using Pim.Data.Repository.Category;
using Pim.Data.Repository.ErrorLogs;
using Pim.Data.Repository.Orders;
using Pim.Data.Repository.Products;
using Pim.Data.Repository.Role;
using Pim.Data.Repository.User;

namespace Pim.Data.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        public IUserRepository UserRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IRoleRepository RoleRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public ICategoryRepository CategoryRepository { get; }
        public IErrorLogRepository ErrorLogRepository { get; }

        private ApplicationDbContext _context;

        public UnitOfWork(
            IUserRepository userRepository,
            IProductRepository productRepository,
            IRoleRepository roleRepository,
            IOrderRepository orderRepository,
            ICategoryRepository categoryRepository,
            IErrorLogRepository errorLogRepository,
            ApplicationDbContext context)
        {
            UserRepository = userRepository;
            ProductRepository = productRepository;
            RoleRepository = roleRepository;
            OrderRepository = orderRepository;
            CategoryRepository = categoryRepository;
            ErrorLogRepository = errorLogRepository;
            _context = context;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task<int> Commit()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
