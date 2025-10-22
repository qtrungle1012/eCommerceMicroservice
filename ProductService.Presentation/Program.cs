using ProductService.Presentation.Configurations;
using ProductService.Presentation.Data;
using ProductService.Presentation.Features.Products.GetProducts;
using SharedLibrarySolution.DependencyInjection;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký MongoDB Context
builder.Services.AddSingleton<MongoDbContext>();

// Đăng ký Handler
builder.Services.AddScoped<GetProductsHandler>();

//Khai báo AutoMapper, tìm MappingProfile trong Assembly(dự án này)
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


// Khởi tạo MongoDB (tạo DB, collections, indexes)
using (var scope = app.Services.CreateScope())
{
    var mongoContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    await MongoDbInitializer.InitializeAsync(mongoContext);
}


// 🔹 Global Exception Middleware
app.UseSharedPoliciesForBackendServices(); // vừa có GlobalException vừa có chặn các request với header k phải gateway


// 🔹 Swagger
app.UseSwaggerDocumentation();

// Chứng thực và phân quyền
app.UseAuthentication();
app.UseAuthorization();
// Map Endpoints

app.MapGetProductsEndpoint();


app.Run();
