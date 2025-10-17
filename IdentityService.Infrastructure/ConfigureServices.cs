using IdentityService.Infrastructure.Data;
using IdentityService.Infrastructure.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrarySolution.DependencyInjection;

namespace IdentityService.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Kết nối database
            var connectionString = configuration.GetConnectionString("IdentityConnection");

            services.AddDbContext<IdentityDbContext>(options =>
                options.UseSqlServer(
                    connectionString,
                    sql => sql
                        .MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName)
                        .MigrationsHistoryTable("__EFMigrationsHistory_Identity")
                )
            );

            // Có thể đăng ký manual nếu muốn
            // services.AddScoped<IUserRepository, UserRepository>();

            services.Configure<JwtSettings>(options =>
            {
                options.SecretKey = configuration["JwtSettings:SecretKey"] ?? "default-secret-key";
                options.Issuer = configuration["JwtSettings:Issuer"] ?? "default-issuer";
                options.Audience = configuration["JwtSettings:Audience"] ?? "default-audience";
                options.AccessTokenExpirationMinutes = int.TryParse(configuration["JwtSettings:AccessTokenExpirationMinutes"], out var m) ? m : 15;
                options.RefreshTokenExpirationDays = int.TryParse(configuration["JwtSettings:RefreshTokenExpirationDays"], out var d) ? d : 7;
            });

            return services;
        }

        public static IApplicationBuilder UseInfrastructurePolicies(this IApplicationBuilder app)
        {
            // Middleware chung (global exception, listen gateway,...)
            SharedServiceContainer.UseSharedPolicies(app);
            return app;
        }
    }
}
