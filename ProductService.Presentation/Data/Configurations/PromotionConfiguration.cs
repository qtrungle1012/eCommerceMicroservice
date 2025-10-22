using MongoDB.Driver;
using ProductService.Presentation.Entities;

namespace ProductService.Presentation.Data.Configurations
{
    public static class PromotionConfiguration
    {
        public static async Task ConfigureAsync(MongoDbContext db)
        {
            // Index theo ngày bắt đầu và kết thúc khuyến mãi
            var dateIndex = new CreateIndexModel<Promotion>(
                Builders<Promotion>.IndexKeys.Ascending(p => p.StartDate).Ascending(p => p.EndDate),
                new CreateIndexOptions { Name = "PromotionDateIndex" });

            await db.Promotions.Indexes.CreateOneAsync(dateIndex);
        }
    }
}
