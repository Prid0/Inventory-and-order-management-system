using Pim.Data.Models;
using Pim.Service.IService;
using System.Diagnostics;

namespace Product_And_Inventory_Mangement.Middleware
{
    public class ApiRequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiRequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            IApiRequestLogService apiRequestLogService)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                var log = new ApiRequestLog
                {
                    RequestPath = context.Request.Path,
                    HttpMethod = context.Request.Method,
                    ResponseStatusCode = context.Response.StatusCode,
                    ElapsedTimeMs = stopwatch.ElapsedMilliseconds,
                    CreatedAt = DateTime.UtcNow
                };
                await apiRequestLogService.AddAsync(log);
            }
        }
    }
}
