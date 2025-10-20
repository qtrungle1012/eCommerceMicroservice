using Autofac;
using Autofac.Extensions.DependencyInjection;
using IdentityService.Application;
using IdentityService.Infrastructure;
using IdentityService.Presentation.Configuration;
using SharedLibrarySolution.DependencyInjection;
using SharedLibrarySolution.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Sử dụng Autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// 2️⃣ Đăng ký DbContext + JwtSettings (Infrastructure)
builder.Services.AddInfrastructureServices(builder.Configuration);

// 3️⃣ Register Autofac container cho Repository, Service, Hasher
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterAssemblyTypes(typeof(IdentityService.Infrastructure.ConfigureServices).Assembly)
        .Where(t => t.Name.EndsWith("Repository") || t.Name.EndsWith("Service") || t.Name.EndsWith("Hasher"))
        .AsImplementedInterfaces()
        .InstancePerLifetimeScope();
});

// 4️⃣ Application layer + API versioning
builder.Services.AddApplicationServices();
builder.Services.AddApiVersioningConfiguration();
builder.Services.AddJWTAuthenticationScheme(builder.Configuration);
builder.Services.AddSwaggerDocumentation();

builder.Services.AddControllers(options =>
{
    options.Conventions.Insert(0, new RoutePrefixConvention("identity"));
});

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// 5️⃣ Middleware chung
app.UseInfrastructurePolicies();

app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<GlobalException>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
