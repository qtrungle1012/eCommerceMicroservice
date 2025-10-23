using FluentValidation;
using ProductService.Presentation.Configurations;
using ProductService.Presentation.Data;
using ProductService.Presentation.Features.Categories.GetCategories;
using ProductService.Presentation.Features.Products.CreateProduct;
using ProductService.Presentation.Features.Products.DeleteProduct;
using ProductService.Presentation.Features.Products.GetProducts;
using ProductService.Presentation.Features.Products.UpdateProduct;
using ProductService.Presentation.Features.Promotions.GetPromotions;
using ProductService.Presentation.Features.Reviews.GetReviews;
using ProductService.Presentation.Services;
using SharedLibrarySolution.DependencyInjection;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký MongoDB Context
builder.Services.AddSingleton<MongoDbContext>();

// Đăng ký Handler
builder.Services.AddScoped<GetProductsHandler>();
builder.Services.AddScoped<GetCategoriesHandler>();
builder.Services.AddScoped<GetPromotionsHandler>();
builder.Services.AddScoped<GetReviewsHandler>();
builder.Services.AddScoped<CreateProductHandler>();
builder.Services.AddScoped<DeleteProductHandler>();
builder.Services.AddScoped<UpdateProductHandler>();


//Khai báo AutoMapper, tìm MappingProfile trong Assembly(dự án này)
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
// Đăng ký tất cả validator
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerDocumentation();
//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<CloudinaryService>(); // service upload ảnh



var app = builder.Build();


// Khởi tạo MongoDB (tạo DB, collections, indexes)
using (var scope = app.Services.CreateScope())
{
    var mongoContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    await MongoDbInitializer.InitializeAsync(mongoContext);
}


// 🔹 Global Exception Middleware
//app.UseSharedPoliciesForBackendServices(); // vừa có GlobalException vừa có chặn các request với header k phải gateway


// 🔹 Swagger
app.UseSwaggerDocumentation();

// Chứng thực và phân quyền
app.UseAuthentication();
app.UseAuthorization();
// Map Endpoints

app.MapGetProductsEndpoint();
app.MapGetCategoriesEndpoint();
app.MapGetPromotionsEndpoint();
app.MapGetReviewsEndpoint();
app.MapCreateProductEndpoint();
app.MapDeleteProductEndpoint();
app.MapUpdateProductEndpoint();



app.Run();
