using Pim.Data.Infrastructure;
using Pim.Data.Models;
using Pim.Model.Dtos;
using Pim.Service.IService;
using Pim.Utility;
using Pim.Utility.SqlHelper;

namespace Pim.Service
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ExecuteSp _executeSp;

        public OrderService(IUnitOfWork unitOfWork, ExecuteSp executeSp)
        {
            _unitOfWork = unitOfWork;
            _executeSp = executeSp;
        }

        public async Task<PagedResult<OrdersResutSet>> GetAllOrders(int from, int to, int userId)
        {
            var totalRecord = 0;
            var fromParameter = DataProvider.GetIntSqlParameter("From", from);
            var toParameter = DataProvider.GetIntSqlParameter("To", to);
            var userIdParameter = DataProvider.GetIntSqlParameter("UserId", userId);
            var totalRecordParameter = DataProvider.GetIntSqlParameter("TotalRecord", totalRecord, true);
            var orders = await _executeSp.ExecuteStoredProcedureListAsync<OrdersResutSet>(
               "GetAllOrders",
               fromParameter,
               toParameter,
               userIdParameter,
               totalRecordParameter
           );

            totalRecord = Convert.ToInt32(totalRecordParameter.Value);

            return new PagedResult<OrdersResutSet>(orders, totalRecord);

        }


        public async Task<OrderDetailsResponse> GetOrderDetails(int orderId)
        {
            var orderIdParam = DataProvider.GetIntSqlParameter("OrderId", orderId);

            var resultSet = await _executeSp
                .ExecuteStoredProcedureListAsync<OrderDetailsResultSet>(
                    "GetOrderDetails",
                    orderIdParam);

            var response = resultSet
                .GroupBy(x => new
                {
                    x.UserId,
                    x.UserName,
                    x.PhoneNumber,
                    x.OrderNumber,
                    x.TotalQuantity,
                    x.OrderTotalValue,
                    x.PlacedOn,
                    x.CancledOn
                })
                .Select(g => new OrderDetailsResponse
                {
                    UserId = g.Key.UserId,
                    UserName = g.Key.UserName,
                    PhoneNumber = g.Key.PhoneNumber,
                    OrderNumber = g.Key.OrderNumber,
                    TotalQuantity = g.Key.TotalQuantity,
                    OrderTotalValue = g.Key.OrderTotalValue,
                    PlacedOn = g.Key.PlacedOn,
                    CancledOn = g.Key.CancledOn,
                    OrderItems = g.Select(item => new OrderItemDetailResponse
                    {
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        LineTotal = item.LineTotal
                    }).ToList()
                })
                .FirstOrDefault();

            return response;

        }


        public async Task<string> AddOrder(int userId, OrderRequest request)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            string result = "error while creating the order";

            try
            {
                if (request.OrderItem == null || request.OrderItem.Count == 0)
                    return "Order must contain at least one item";

                var newOrder = new Order
                {
                    UserId = userId,
                    PlacedOn = DateTime.UtcNow,
                    IsActive = true
                };

                await _unitOfWork.OrderRepository.Add(newOrder);
                await _unitOfWork.Commit();

                var activeProducts = (await _unitOfWork.ProductRepository.GetAll(x => x.IsActive));

                foreach (var item in request.OrderItem)
                {
                    if (item.Quantity <= 0)
                        return "Quantity must be greater than zero";

                    var product = activeProducts.FirstOrDefault(p => p.Id == item.ProductId);

                    if (product == null || item.Quantity > product.Quantity)
                        return $"The product '{product.Name}' is currently unavailable or out of stock.";

                    var orderItem = new OrderItem
                    {
                        OrderId = newOrder.Id,
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price,
                        TotalPrice = item.Quantity * product.Price
                    };

                    product.Quantity -= item.Quantity;

                    await _unitOfWork.ProductRepository.Update(product);
                    await _unitOfWork.OrderRepository.AddOrderItems(orderItem);
                }

                await _unitOfWork.Commit();
                await transaction.CommitAsync();
                result = "success";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                result = ex.Message;
            }

            return result;
        }


        public async Task<string> DeleteOrder(int orderId)
        {
            string result = "error while deleting the order";

            try
            {
                var order = await _unitOfWork.OrderRepository.GetById(orderId);

                if (order == null || !order.IsActive)
                {
                    result = "Order not found or already inactive";
                    return result;
                }

                order.IsActive = false;
                order.CancledOn = DateTime.UtcNow;

                await _unitOfWork.OrderRepository.Update(order);
                await _unitOfWork.Commit();
                result = "success";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }
    }
}
