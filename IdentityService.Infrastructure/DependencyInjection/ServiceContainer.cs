using IdentityService.Application.Interfaces;
using IdentityService.Infrastructure.Data;
using IdentityService.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrarySolution.DependencyInjection;

namespace IdentityService.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            // kết nôi database
            var connectionString = configuration.GetConnectionString("IdentityConnection");

            services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlServer(
                connectionString,
                sql => sql
                    .MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName)
                    .MigrationsHistoryTable("__EFMigrationsHistory_Identity")
            ));


            // DI (có thể đổi thành auto DI)
            //services.AddScoped < IUserRepository, UserRepository>();

            return services;
        }
        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            // đăng ký middleware: global exception, listen gateway
            SharedServiceContainer.UseSharedPolicies(app);
            return app;
        }
    }
}
