using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pim.Model.Dtos;
using Pim.Service;
using Pim.Utility;

namespace Product_And_Inventory_Mangement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseApiController
    {
        private readonly UserService _userService;

        public UsersController(UserService userService, LoggedInUserId loggedInUserId) : base(loggedInUserId)
        {
            _userService = userService;
        }

        [HttpPost("AddOrUpdate")]
        public async Task<IActionResult> AddOrUpdateUser([FromBody] UserRequest request)
        {
            var result = await _userService.AddOrUpdateUser(userId, request);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers(int from, int to)
        {
            var users = await _userService.GetAllUsers(from, to);
            return Ok(users);
        }

        [Authorize(Roles = "Admin,Manager,Customer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUsersById(id);

            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }

        [Authorize(Roles = "Admin,Customer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUser(userId, id);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Customer")]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var result = await _userService.ResetPassword(request);
            return Ok(result);
        }
    }
}
