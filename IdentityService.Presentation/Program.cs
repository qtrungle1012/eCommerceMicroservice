using Autofac;
using Autofac.Extensions.DependencyInjection;
using IdentityService.Application;
using IdentityService.Infrastructure;
using IdentityService.Presentation.Configuration;
using SharedLibrarySolution.DependencyInjection;
using SharedLibrarySolution.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Dùng Autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// 🔹 Đăng ký các tầng
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddJWTAuthenticationScheme(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddSwaggerDocumentation();

// 🔹 Autofac Container
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterAssemblyTypes(typeof(IdentityService.Infrastructure.ConfigureServices).Assembly)
        .Where(t => t.Name.EndsWith("Repository") || t.Name.EndsWith("Service") || t.Name.EndsWith("Hasher"))
        .AsImplementedInterfaces()
        .InstancePerLifetimeScope();
});

var app = builder.Build();

// 🔹 Global Exception Middleware
app.UseMiddleware<GlobalException>();

// 🔹 Swagger
app.UseSwaggerDocumentation();

// 🔹 Auth
app.UseAuthentication();
app.UseAuthorization();

// 🔹 Map Controllers
app.MapControllers();

app.Run();
