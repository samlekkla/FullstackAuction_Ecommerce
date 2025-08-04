using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AuctionCommerce.Middleware
{
    public class CorsDebugMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CorsDebugMiddleware> _logger;

        public CorsDebugMiddleware(RequestDelegate next, ILogger<CorsDebugMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var origin = context.Request.Headers["Origin"].ToString();
            var method = context.Request.Method;
            var path = context.Request.Path;

            // Logga alla CORS-relaterade requests
            if (!string.IsNullOrEmpty(origin))
            {
                _logger.LogInformation("CORS Request: {Method} {Path} from origin: {Origin}", method, path, origin);
            }

            // Hantera preflight requests manuellt
            if (method == "OPTIONS")
            {
                _logger.LogInformation("Handling preflight request for {Path} from {Origin}", path, origin);
                
                context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE, PATCH, OPTIONS";
                context.Response.Headers["Access-Control-Allow-Headers"] = 
                    "Content-Type, Authorization, Accept, Origin, X-Requested-With, Accept-Language, Content-Language";
                context.Response.Headers["Access-Control-Max-Age"] = "86400";
                
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync("");
                return;
            }

            await _next(context);

            // Logga response headers för debugging
            if (!string.IsNullOrEmpty(origin))
            {
                var corsHeader = context.Response.Headers["Access-Control-Allow-Origin"].ToString();
                _logger.LogInformation("Response CORS header: {CorsHeader} for origin: {Origin}", corsHeader, origin);
            }
        }
    }

    public static class CorsDebugMiddlewareExtensions
    {
        public static IApplicationBuilder UseCorsDebugMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorsDebugMiddleware>();
        }
    }
}
