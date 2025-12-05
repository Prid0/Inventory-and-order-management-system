using Pim.Data.Infrastructure;
using Pim.Data.Models;
using Pim.Model.Dtos;
using Pim.Utility;
using Pim.Utility.SqlHelper;

namespace Pim.Service
{
    public class OrderService
    {
        private readonly IUnitOfWork _uow;
        private readonly ExecuteSp _executeSp;
        private readonly LoggedInUserId _loggedInUserId;

        public OrderService(IUnitOfWork uow, ExecuteSp executeSp, LoggedInUserId loggedInUserId)
        {
            _uow = uow;
            _executeSp = executeSp;
            _loggedInUserId = loggedInUserId;
        }

        public async Task<List<OrdersResponse>> GetAllOrders()
        {
            var result = await _executeSp.ExecuteStoredProcedureListAsync<OrdersResutSet>(
               "GetAllOrders"

           );
            var orders = result.Select(x => new OrdersResponse
            {
                UserName = x.UserName,
                PhoneNumber = x.PhoneNumber,
                OrderNumber = x.OrderNumber,
                OrderTotalValue = x.OrderTotalValue,
                PlacedOn = x.PlacedOn.ToString("dd-MM-yyyy"),
                CancledOn = x.CancledOn.HasValue ? x.CancledOn.Value.ToString("dd-MM-yyyy") : ""
            }).ToList();
            return orders;
        }

        public async Task<OrderDetailsResponse> GetOrderDetails(int orderId)
        {
            var order = await _uow.OrderRepository.GetById(orderId);

            if (order == null || !order.IsActive)
            {
                return null;
            }

            var orderIdParam = DataProvider.GetIntSqlParameter("OrderId", orderId);

            var result = await _executeSp.ExecuteStoredProcedureListAsync<OrderDetailsResultSet>(
                "GetOrderDetails",
                orderIdParam
            );

            if (result == null || result.Count == 0)
                return null;

            var response = result
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
                    PlacedOn = g.Key.PlacedOn.ToString("dd-MM-yyyy"),
                    CancledOn = g.Key.CancledOn.HasValue ? g.Key.CancledOn.Value.ToString("dd-MM-yyyy") : "",

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

        public async Task<string> AddOrder(OrderRequest request)
        {
            using var transaction = await _uow.BeginTransactionAsync();
            string result = "error while creating the order";

            try
            {
                var loginData = _loggedInUserId.GetUserAndRole();
                var validUser = await _uow.UserRepository.GetById(loginData.userId);

                if (validUser == null || !validUser.IsActive)
                    return "Invalid or inactive user";

                if (request.OrderItem == null || request.OrderItem.Count == 0)
                    return "Order must contain at least one item";

                var newOrder = new Order
                {
                    UserId = loginData.userId,
                    PlacedOn = DateTime.UtcNow,
                    IsActive = true
                };

                await _uow.OrderRepository.Add(newOrder);
                await _uow.Commit();

                var activeProducts = (await _uow.ProductRepository.GetAll())
                                        .Where(p => p.IsActive)
                                        .ToList();

                foreach (var item in request.OrderItem)
                {
                    if (item.Quantity <= 0)
                        return "Quantity must be greater than zero";

                    var product = activeProducts.FirstOrDefault(p => p.Id == item.ProductId);

                    if (product == null || item.Quantity > product.Quantity)
                        return $"Invalid product: {item.ProductId} or out of stock";

                    var orderItem = new OrderItem
                    {
                        OrderId = newOrder.Id,
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price,
                        TotalPrice = item.Quantity * product.Price
                    };

                    product.Quantity -= item.Quantity;

                    await _uow.ProductRepository.Update(product);
                    await _uow.OrderRepository.AddOrderItems(orderItem);
                }

                await _uow.Commit();
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
                var order = await _uow.OrderRepository.GetById(orderId);

                if (order == null || !order.IsActive)
                {
                    result = "Order not found or already inactive";
                    return result;
                }

                order.IsActive = false;
                order.CancledOn = DateTime.UtcNow;

                await _uow.OrderRepository.Update(order);
                await _uow.Commit();

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
