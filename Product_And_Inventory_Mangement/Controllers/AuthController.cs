using Microsoft.AspNetCore.Mvc;
using Pim.Model.Dtos;
using Pim.Service;

namespace Pim.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.Login(request.Email, request.Password);

            if (!result.Success)
                return Unauthorized("Invalid email or password");

            return Ok(new
            {
                token = result.Token,
                userId = result.UserId,
                role = result.Role
            });
        }
    }
}
