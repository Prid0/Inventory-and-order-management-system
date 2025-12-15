using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pim.Model.Dtos;
using Pim.Service;
using Pim.Utility;

namespace Product_And_Inventory_Mangement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly LoggedInUserId _loggedInUserId;

        public ProductsController(ProductService productService, LoggedInUserId loggedInUserId)
        {
            _productService = productService;
            _loggedInUserId = loggedInUserId;
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost("AddOrUpdate")]
        public async Task<IActionResult> AddOrUpdateProduct([FromBody] ProductRequest request)
        {
            var (userId, roleId) = _loggedInUserId.GetUserAndRole();
            var result = await _productService.AddOrUpdateProduct(userId, request);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Manager,Customer")]
        [HttpGet]
        public async Task<IActionResult> GetAllProducts(int from, int to)
        {
            var products = await _productService.GetAllProducts(from, to);
            return Ok(products);
        }

        [Authorize(Roles = "Admin,Manager,Customer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdProduct(int id)
        {
            var product = await _productService.GetProductById(id);

            if (product == null)
                return NotFound("Product not found");

            return Ok(product);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var (userId, roleId) = _loggedInUserId.GetUserAndRole();
            var result = await _productService.DeleteProduct(userId, id);
            return Ok(result);
        }
    }
}
