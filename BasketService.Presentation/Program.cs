using BasketService.Presentation.Configurations;
using BasketService.Presentation.Features.Basket.GetBasket;
using BasketService.Presentation.Features.Baskets.AddItemToBasket;
using BasketService.Presentation.Features.Baskets.Consumers;
using BasketService.Presentation.Features.Baskets.GetBasket;
using MassTransit;
using SharedLibrarySolution.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

//JWT Authentication
builder.Services.AddJWTAuthenticationScheme(builder.Configuration);
builder.Services.AddHttpContextAccessor();


// consummer nhận even tu producer
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProductUpdatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });

        // ✅ SỬA LẠI - Thêm RoutingKey
        cfg.ReceiveEndpoint("basket_product_update_queue", e =>
        {
            e.ConfigureConsumeTopology = false;

            e.Bind("product_exchange", x => // exchange name
            {
                x.ExchangeType = "direct";
                x.RoutingKey = "product.updated"; // ✅ THÊM ROUTING KEY
            });

            e.ConfigureConsumer<ProductUpdatedConsumer>(context);
        });
    });
});


// Add services to the container.
//Khai báo AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Basket Handlers
builder.Services.AddScoped<AddItemHandler>();
builder.Services.AddScoped<GetBasketHandler>();


builder.Services.AddControllers();
builder.Services.AddSwaggerDocumentation();

// cấu hình redis
builder.Services.AddRedisConfiguration(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

// 🔹 Swagger
app.UseSwaggerDocumentation();
// Chứng thực và phân quyền
app.UseAuthentication();
app.UseAuthorization();

// Basket Endpoints
app.MapAddItemEndpoint();
app.MapGetBasketEndpoint();



app.Run();
