using Microsoft.AspNetCore.Http;
using SharedLibrarySolution.Logs;
using System.Net;
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
                switch (context.Response.StatusCode)
                {
                    case StatusCodes.Status401Unauthorized:
                        await ModifyHeader(context, "Unauthorized", "Authentication required or invalid token.", StatusCodes.Status401Unauthorized);
                        break;

                    case StatusCodes.Status403Forbidden:
                        await ModifyHeader(context, "Forbidden", "You do not have permission to access this resource.", StatusCodes.Status403Forbidden);
                        break;

                    case StatusCodes.Status404NotFound:
                        await ModifyHeader(context, "Not Found", "The requested resource could not be found.", StatusCodes.Status404NotFound);
                        break;

                    case StatusCodes.Status429TooManyRequests:
                        await ModifyHeader(context, "Warning", "Too many requests !", StatusCodes.Status429TooManyRequests);
                        break;
                }
            }
            catch (Exception ex)
            {
                LogException.LogError(ex);

                await HandleExceptionAsync(context, ex);

                if (ex is TaskCanceledException || ex is TimeoutException)
                {
                    await ModifyHeader(context, "Out of time", "Request time out !", StatusCodes.Status408RequestTimeout);
                }
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                statusCode = context.Response.StatusCode,
                title = "Error",
                message = exception.Message // có thể thay = "Internal server error"
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }

        private static async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

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
