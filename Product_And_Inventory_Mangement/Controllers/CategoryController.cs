using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pim.Model.Dtos;
using Pim.Service;

namespace Product_And_Inventory_Mangement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ProductCategoryService _categoryService;

        public CategoriesController(ProductCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [Authorize(Roles = "1,2")]
        [HttpPost("AddOrUpdate")]
        public async Task<IActionResult> AddOrUpdateCategory([FromBody] CategoryRequest request)
        {
            var result = await _categoryService.AddOrUpdateCategory(request);
            return Ok(result);
        }

        [Authorize(Roles = "1,2")]
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategory();
            return Ok(categories);
        }

        [Authorize(Roles = "1,2")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryById(id);

            if (category == null)
                return NotFound("Category not found");

            return Ok(category);
        }

        [Authorize(Roles = "1,2")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategory(id);
            return Ok(result);
        }
    }
}
