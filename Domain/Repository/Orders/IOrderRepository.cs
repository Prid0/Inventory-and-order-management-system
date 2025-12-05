using Pim.Data.Infrastructure;
using Pim.Data.Models;

namespace Pim.Data.Repository.Orders
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task AddOrderItems(OrderItem items);
    }
}
