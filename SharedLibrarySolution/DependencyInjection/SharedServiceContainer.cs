using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SharedLibrarySolution.Middleware;

namespace SharedLibrarySolution.DependencyInjection
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedServices<TContext>(this IServiceCollection services,
                    IConfiguration configuration,
                    string fileName) where TContext : DbContext
        {
            // add database context
            services.AddDbContext<TContext>(option => option.UseSqlServer(
                configuration.GetConnectionString("eCommerceConnection"), sqlserverOption =>
                sqlserverOption.EnableRetryOnFailure()
            ));
            // serilog config
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.Debug()
                .WriteTo.File($"{fileName}_logs.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            // logging to service container
            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(dispose: true)
            );

            JWTAuthenticationScheme.AddJWTAuthenticationScheme(services, configuration);

            return services;
        }

        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalException>();

            app.UseMiddleware<ListenToOnlyApiGateway>();

            return app;
        }

    }
}
