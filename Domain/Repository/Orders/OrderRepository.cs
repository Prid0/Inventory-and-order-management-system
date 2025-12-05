using Pim.Data.Infrastructure;
using Pim.Data.Models;

namespace Pim.Data.Repository.Orders
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddOrderItems(OrderItem items)
        {
            await _context.OrderItem.AddAsync(items);
        }
    }
}
