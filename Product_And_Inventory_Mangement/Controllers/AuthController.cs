using Microsoft.AspNetCore.Mvc;
using Pim.Model.Dtos;
using Pim.Service.IService;

namespace Pim.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.Login(request.Email, request.Password);

            if (!result.Success)
                return Unauthorized("Invalid email or password");

            return Ok(result);
        }
    }
}
