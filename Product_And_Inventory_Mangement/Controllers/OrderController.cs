using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pim.Model.Dtos;
using Pim.Service;

namespace Product_And_Inventory_Mangement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize(Roles = "3")]
        [HttpPost]
        public async Task<IActionResult> AddAOrder([FromBody] OrderRequest request)
        {
            var result = await _orderService.AddOrder(request);
            return Ok(result);
        }

        [Authorize(Roles = "1,2")]
        [HttpGet]
        public async Task<IActionResult> GetAllOrders(int from, int to, int userId)
        {
            var orders = await _orderService.GetAllOrders(from, to, userId);
            return Ok(orders);
        }

        [Authorize(Roles = "1,2,3")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var result = await _orderService.GetOrderDetails(id);

            if (result == null)
                return NotFound("Order not found");

            return Ok(result);
        }

        [Authorize(Roles = "3")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await _orderService.DeleteOrder(id);
            return Ok(result);
        }
    }
}
