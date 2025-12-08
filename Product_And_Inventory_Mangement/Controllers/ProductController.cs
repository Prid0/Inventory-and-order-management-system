using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pim.Model.Dtos;
using Pim.Service;

namespace Product_And_Inventory_Mangement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [Authorize(Roles = "1,2")]
        [HttpPost("AddOrUpdate")]
        public async Task<IActionResult> AddOrUpdateProduct([FromBody] ProductRequest request)
        {
            var result = await _productService.AddOrUpdateProduct(request);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts(int from, int to)
        {
            var products = await _productService.GetAllProducts(from, to);
            return Ok(products);
        }

        [Authorize(Roles = "1,2")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdProduct(int id)
        {
            var product = await _productService.GetProductById(id);

            if (product == null)
                return NotFound("Product not found");

            return Ok(product);
        }

        [Authorize(Roles = "1,2")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProduct(id);
            return Ok(result);
        }
    }
}
