using Pim.Data;
using Pim.Data.Models;
using Pim.IRepository;
using Pim.Repository.IRepository;

namespace Pim.Repository
{

    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IgenericRepo<Users> Users { get; private set; }
        public IgenericRepo<Roles> Roles { get; private set; }
        public IgenericRepo<Product> Products { get; private set; }
        public IgenericRepo<Order> Orders { get; private set; }
        public IgenericRepo<ProductCategory> Categories { get; private set; }
        public IgenericRepo<UserRoleMapping> UserRoleMapping { get; private set; }
        public IgenericRepo<OrderItem> OrderItem { get; private set; }
        public IgenericRepo<ErrorLog> ErrorLog { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Users = new GenericRepo<Users>(_context);
            Roles = new GenericRepo<Roles>(_context);
            Products = new GenericRepo<Product>(_context);
            Orders = new GenericRepo<Order>(_context);
            Categories = new GenericRepo<ProductCategory>(_context);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

