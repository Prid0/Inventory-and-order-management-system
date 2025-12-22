using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pim.Model.Dtos;
using Pim.Service.IService;
using Pim.Utility;

namespace Product_And_Inventory_Mangement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService, LoggedInUserId loggedInUserId) : base(loggedInUserId)
        {
            _orderService = orderService;
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> AddAOrder([FromBody] OrderRequest request)
        {
            var result = await _orderService.AddOrder(userId, request);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        public async Task<IActionResult> GetAllOrders(int from, int to, int userId)
        {
            var orders = await _orderService.GetAllOrders(from, to, userId);
            return Ok(orders);
        }

        [Authorize(Roles = "Admin,Manager,Customer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var result = await _orderService.GetOrderDetails(id);

            if (result == null)
                return NotFound("Order not found");

            return Ok(result);
        }

        [Authorize(Roles = "Customer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await _orderService.DeleteOrder(id);
            return Ok(result);
        }
    }
}
