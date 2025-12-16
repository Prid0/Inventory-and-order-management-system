using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Pim.Utility
{
    public class LoggedInUserId
    {
        private readonly IHttpContextAccessor _http;

        public LoggedInUserId(IHttpContextAccessor http)
        {
            _http = http;
        }
        public int GetUserId()
        {
            var userClaim = _http.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            //var roleClaim = _http.HttpContext?.User?.FindFirst(ClaimTypes.Role);

            int userId = userClaim != null ? int.Parse(userClaim.Value) : 0;
            //int roleId = roleClaim != null ? int.Parse(roleClaim.Value) : 0;

            return userId;
        }

    }
}
