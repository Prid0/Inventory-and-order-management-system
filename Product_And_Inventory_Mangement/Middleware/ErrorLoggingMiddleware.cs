using Pim.Data.Models;
using Pim.Service;
using Pim.Utility;
using System.Diagnostics;
using System.Security.Claims;

public class ErrorLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // Create a new scope for logging
            using (var scope = context.RequestServices.CreateScope())
            {
                var errorLogsService = scope.ServiceProvider.GetRequiredService<ErrorLogsService>();
                var loggedInUserId = scope.ServiceProvider.GetRequiredService<LoggedInUserId>();

                var loggedInData = loggedInUserId.GetUserAndRole();

                if (context.User.Identity?.IsAuthenticated == true)
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

            // Exception swallowed; request continues
        }
    }
}
