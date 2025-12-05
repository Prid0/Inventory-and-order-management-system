using Pim.Data.Models;
using Pim.Service;
using Pim.Utility;
using System.Diagnostics;
using System.Security.Claims;


namespace Pim.Api.Middleware
{
    public class ErrorLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly LoggedInUserId _loggedInUserId;
        public ErrorLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ErrorLogsService errorLogsService, LoggedInUserId loggedInUserId)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var loggedInData = _loggedInUserId.GetUserAndRole();
                stopwatch.Stop();

                if (context.User.Identity!.IsAuthenticated)
                {
                    var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    if (!string.IsNullOrEmpty(userIdClaim))
                        loggedInData.userId = int.Parse(userIdClaim);
                }

                var log = new ErrorLog
                {
                    RequestPath = context.Request.Path,
                    HTTPMethod = context.Request.Method,
                    ResponseStatusCode = context.Response.StatusCode.ToString(),
                    ErrorMessage = ex.Message,
                    ExecutionTime = stopwatch.ElapsedMilliseconds + " ms",
                    MadeBy = loggedInData.userId
                };

                await errorLogsService.AddAsync(log);
            }

        }
    }
}
