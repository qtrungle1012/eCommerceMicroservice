using Microsoft.AspNetCore.Http;
using SharedLibrarySolution.Logs;
using System.Text.Json;

namespace SharedLibrarySolution.Middleware
{
    public class GlobalException
    {
        private readonly RequestDelegate _next;

        public GlobalException(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                // ✅ Chỉ ghi response nếu chưa started
                if (!context.Response.HasStarted)
                {
                    switch (context.Response.StatusCode)
                    {
                        case StatusCodes.Status401Unauthorized:
                            await WriteResponse(context, 401, "Unauthorized",
                                "Authentication required or invalid token.");
                            break;

                        case StatusCodes.Status403Forbidden:
                            await WriteResponse(context, 403, "Forbidden",
                                "You do not have permission to access this resource.");
                            break;

                        case StatusCodes.Status404NotFound:
                            await WriteResponse(context, 404, "Not Found",
                                "The requested resource could not be found.");
                            break;

                        case StatusCodes.Status429TooManyRequests:
                            await WriteResponse(context, 429, "Warning",
                                "Too many requests!");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogException.LogError(ex);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var cid = context.Items["X-Correlation-ID"]?.ToString();
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    statusCode = 500,
                    title = "Internal Server Error",
                    correlationId = cid,
                    traceId = context.TraceIdentifier,
                    detail = exception.Message
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
            }
        }


        private static async Task WriteResponse(HttpContext context, int statusCode, string title, string message)
        {
            if (context.Response.HasStarted)
                return;

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                statusCode,
                title,
                message
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}