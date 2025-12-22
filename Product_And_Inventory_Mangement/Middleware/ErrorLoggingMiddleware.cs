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
            if (context.Response.HasStarted)
                throw;

            stopwatch.Stop();

            using var scope = context.RequestServices.CreateScope();
            var errorLogsService = scope.ServiceProvider
                .GetRequiredService<IErrorLogsService>();

            int statusCode = ex switch
            {
                ArgumentNullException => StatusCodes.Status400BadRequest,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var log = new ErrorLog
            {
                RequestPath = context.Request.Path,
                HTTPMethod = context.Request.Method,
                ResponseStatusCode = statusCode.ToString(),
                ErrorMessage = ex.Message,
                StackTrace = ex.StackTrace ?? string.Empty,
                ExecutionTime = $"{stopwatch.ElapsedMilliseconds} ms",
                CreatedDate = DateTime.UtcNow
            };

            await errorLogsService.AddAsync(log);

            var response = new
            {
                success = false,
                message = ex.Message,
                status = statusCode
            };

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
        finally
        {
            stopwatch.Stop();
        }
    }
}
