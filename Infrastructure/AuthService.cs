using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Pim.Data.Infrastructure;
using Pim.Model.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Pim.Service
{
    public class AuthService
    {
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _config;

        public AuthService(IUnitOfWork uow, IConfiguration config)
        {
            _uow = uow;
            _config = config;
        }

        public async Task<LoginResponse> Login(string email, string password)
        {
            var user = await _uow.UserRepository.GetUserByEmail(email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                return new LoginResponse { Success = false, Token = "", UserId = 0, RoleId = 0 };

            var role = await _uow.UserRepository.GetRoleMappingById(user.Id);

            string token = GenerateJwtToken(user.Id, role.RoleId);

            return new LoginResponse
            {
                Success = true,
                Token = token,
                UserId = user.Id,
                RoleId = role.RoleId
            };
        }

        private string GenerateJwtToken(int userId, int roleId)
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var creds = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, roleId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(double.Parse(_config["Jwt:ExpiresInHours"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
