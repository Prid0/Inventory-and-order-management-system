using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pim.Model.Dtos;
using Pim.Service.IService;
using Pim.Utility;

namespace Product_And_Inventory_Mangement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : BaseApiController
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService, LoggedInUserId loggedInUserId) : base(loggedInUserId)
        {
            _roleService = roleService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddOrUpdate")]
        public async Task<IActionResult> AddOrUpdateRole([FromBody] RoleRequest request)
        {
            var result = await _roleService.AddOrUpdateRole(userId, request);

            return Ok(result);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _roleService.GetAllRoles();
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            var result = await _roleService.GetRolesById(id);

            return result != null ? Ok(result) : NotFound();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var result = await _roleService.DeleteRole(userId, id);

            return result != null ? Ok(result) : NotFound();
        }
    }
}
