using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Text.Json;

namespace SharedLibrarySolution.Logs
{
    public static class LogException
    {
        public static async Task HandleRequestAsync(HttpContext context, RequestDelegate next)
        {
            var sw = Stopwatch.StartNew();
            var cid = context.Items["X-Correlation-ID"]?.ToString();

            try
            {
                await next(context);
                sw.Stop();
                Log.Information("➡️ {Method} {Path} -> {Status} ({Elapsed} ms) cid={Cid}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    sw.ElapsedMilliseconds,
                    cid);
            }
            catch (Exception ex)
            {
                sw.Stop();
                LogError(ex, context.Request, sw.ElapsedMilliseconds, cid);
                
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500;

                var response = new
                {
                    statusCode = 500,
                    title = "Internal Server Error",
                    correlationId = cid,
                    traceId = context.TraceIdentifier,
                    detail = ex.Message
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
            }
        }

        public static void LogError(Exception ex, HttpRequest? request = null, long? elapsedMs = null, string? correlationId = null)
        {
            var msg = ex.Message;

            if (request != null)
                msg = $"{request.Method} {request.Path} failed after {elapsedMs} ms";

            if (correlationId != null)
                msg += $" | cid={correlationId}";

            LogToFile(msg);
            LogToConsole(msg);
            LogToDebugger(msg);
        }

        public static void LogToFile(string message) => Log.Information($"{DateTime.Now}: {message}");
        private static void LogToConsole(string message) => Log.Information($"{DateTime.Now}: {message}");
        private static void LogToDebugger(string message) => Log.Information($"{DateTime.Now}: {message}");
    }
}
