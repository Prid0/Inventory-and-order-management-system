using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pim.Model.Dtos;
using Pim.Service;
using Pim.Utility;

namespace Product_And_Inventory_Mangement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : BaseApiController
    {
        private readonly ProductCategoryService _categoryService;

        public CategoriesController(ProductCategoryService categoryService, LoggedInUserId loggedInUserId) : base(loggedInUserId)
        {
            _categoryService = categoryService;
        }


        [Authorize(Roles = "Admin,Manager")]
        [HttpPost("AddOrUpdate")]
        public async Task<IActionResult> AddOrUpdateCategory([FromBody] CategoryRequest request)
        {
            var result = await _categoryService.AddOrUpdateCategory(userId, request);
            return Ok(result);
        }


        [Authorize(Roles = "Admin,Manager,Customer")]
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategory();
            return Ok(categories);
        }


        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryById(id);

            if (category == null)
                return NotFound("Category not found");

            return Ok(category);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategory(userId, id);
            return Ok(result);
        }
    }
}
