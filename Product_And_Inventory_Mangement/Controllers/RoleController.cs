using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pim.Model.Dtos;
using Pim.Service;
using Pim.Utility;

namespace Product_And_Inventory_Mangement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly RoleService _roleService;
        private readonly LoggedInUserId _loggedInUserId;

        public RolesController(RoleService roleService, LoggedInUserId loggedInUserId)
        {
            _roleService = roleService;
            _loggedInUserId = loggedInUserId;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddOrUpdate")]
        public async Task<IActionResult> AddOrUpdateRole([FromBody] RoleRequest request)
        {
            var (userId, roleId) = _loggedInUserId.GetUserAndRole();
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

            if (result != null)
                return Ok(result);

            return NotFound();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var (userId, roleId) = _loggedInUserId.GetUserAndRole();
            var result = await _roleService.DeleteRole(userId, id);

            if (result != null)
                return Ok(result);

            return NotFound();
        }
    }
}
