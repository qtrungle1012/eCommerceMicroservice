using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace SharedLibrarySolution.Middleware
{
    public class ListenToOnlyApiGateway
    {
        private readonly RequestDelegate _next;
        private const string GatewayHeader = "Api-Gateway";
        private const string ExpectedHeaderValue = "my-gateway";

        public ListenToOnlyApiGateway(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var headerValue = context.Request.Headers[GatewayHeader].FirstOrDefault();

            // null la nhung request co header khong den API gateway - 503
            if (string.IsNullOrEmpty(headerValue) || headerValue != ExpectedHeaderValue)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    statusCode = context.Response.StatusCode,
                    title = "Service Unavailable",
                    message = "This service can only be accessed through the API Gateway."
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                return;
            }

            await _next(context);
        }
    }
}
