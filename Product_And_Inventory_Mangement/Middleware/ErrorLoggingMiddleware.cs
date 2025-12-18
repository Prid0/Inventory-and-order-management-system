using Pim.Data.Models;
using Pim.Service.IService;
using System.Diagnostics;
using System.Text.Json;

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

            if (context.Response.HasStarted)
                throw;

            context.Response.Clear();
            context.Response.ContentType = "application/json";

            int statusCode = StatusCodes.Status500InternalServerError;

            switch (ex)
            {
                case ArgumentNullException:
                    statusCode = StatusCodes.Status400BadRequest;
                    break;
                case UnauthorizedAccessException:
                    statusCode = StatusCodes.Status401Unauthorized;
                    break;
                case KeyNotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    break;
                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    break;
            }

            context.Response.StatusCode = statusCode;

            var log = new ErrorLog
            {
                RequestPath = context.Request.Path,
                HTTPMethod = context.Request.Method,
                ResponseStatusCode = statusCode.ToString(),
                ErrorMessage = ex.Message,
                StackTrace = ex.StackTrace,
                ExecutionTime = $"{stopwatch.ElapsedMilliseconds} ms"
            };

            await errorLogsService.AddAsync(log);

            var response = new
            {
                success = false,
                message = ex.Message,
                status = statusCode
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));

            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }
}
