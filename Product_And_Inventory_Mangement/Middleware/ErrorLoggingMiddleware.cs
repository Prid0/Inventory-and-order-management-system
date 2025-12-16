using Pim.Data.Models;
using Pim.Service.IService;
using System.Diagnostics;

public class ErrorLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            using var scope = context.RequestServices.CreateScope();
            var errorLogsService = scope.ServiceProvider
                .GetRequiredService<IErrorLogsService>();

            var log = new ErrorLog
            {
                RequestPath = context.Request.Path,
                HTTPMethod = context.Request.Method,
                ResponseStatusCode = StatusCodes
                    .Status500InternalServerError.ToString(),
                ErrorMessage = ex.Message,
                StackTrace = ex.StackTrace,
                ExecutionTime = $"{stopwatch.ElapsedMilliseconds} ms"
            };

            await errorLogsService.AddAsync(log);

            throw;
        }
    }
}
