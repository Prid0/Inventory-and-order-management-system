using Pim.Model.Dtos;
using Pim.Utility;

namespace Pim.Service.IService
{
    public interface IOrderService
    {
        Task<PagedResult<OrdersResutSet>> GetAllOrders(int from, int to, int userId);

        Task<OrderDetailsResponse> GetOrderDetails(int orderId);

        Task<string> AddOrder(int userId, OrderRequest request);

        Task<string> DeleteOrder(int orderId);
    }
}
