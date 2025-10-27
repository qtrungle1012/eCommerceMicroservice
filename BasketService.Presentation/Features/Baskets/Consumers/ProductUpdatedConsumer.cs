using BasketService.Presentation.Entities.Events;
using MassTransit;
using StackExchange.Redis;
using System.Text.Json;

namespace BasketService.Presentation.Features.Baskets.Consumers
{
    public class ProductUpdatedConsumer : IConsumer<ProductUpdatedEvent>
    {
        private readonly IDatabase _redis;
        private readonly ILogger<ProductUpdatedConsumer> _logger;

        public ProductUpdatedConsumer(IConnectionMultiplexer redis, ILogger<ProductUpdatedConsumer> logger)
        {
            _redis = redis.GetDatabase();
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ProductUpdatedEvent> context)
        {
            var @event = context.Message;

            _logger.LogInformation("📦 Received ProductUpdatedEvent: ProductId={ProductId}, Price={Price}",
                @event.ProductId, @event.Price);

            try
            {
                var productUsersKey = $"product:{@event.ProductId}:users";
                var userIds = await _redis.SetMembersAsync(productUsersKey);

                if (userIds.Length == 0)
                {
                    _logger.LogInformation("No baskets contain ProductId={ProductId}", @event.ProductId);
                    return;
                }

                var updatedCount = 0;

                foreach (var userId in userIds)
                {
                    var basketKey = $"basket:{userId}";
                    var basketJson = await _redis.StringGetAsync(basketKey);

                    if (string.IsNullOrEmpty(basketJson))
                    {
                        await _redis.SetRemoveAsync(productUsersKey, userId);
                        continue;
                    }

                    var basket = JsonSerializer.Deserialize<BasketService.Presentation.Entities.Basket>(basketJson!);
                    if (basket == null) continue;

                    // ✅ CHỈ update items KHÔNG có SKU (sản phẩm đơn giản)
                    var itemsToUpdate = basket.Items
                        .Where(i => i.ProductId == @event.ProductId && string.IsNullOrEmpty(i.Sku))
                        .ToList();

                    if (itemsToUpdate.Any())
                    {
                        foreach (var item in itemsToUpdate)
                        {
                            item.Price = @event.Price;
                            // DiscountPrice giữ nguyên
                        }

                        basket.UpdatedAt = DateTime.UtcNow;

                        var updatedJson = JsonSerializer.Serialize(basket);
                        await _redis.StringSetAsync(basketKey, updatedJson, basket.TTL);

                        updatedCount++;
                    }
                }

                _logger.LogInformation("✅ Updated {Count} baskets for ProductId={ProductId}",
                    updatedCount, @event.ProductId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error processing ProductUpdatedEvent");
                throw;
            }
        }
    }
}