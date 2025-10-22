using MongoDB.Driver;
using ProductService.Presentation.Entities;

namespace ProductService.Presentation.Data.Configurations
{
    public static class CategoryConfiguration
    {
        public static async Task ConfigureAsync(MongoDbContext db)
        {
            // Tạo index để tìm kiếm danh mục con theo ParentId
            var parentIndex = new CreateIndexModel<Category>(
                Builders<Category>.IndexKeys.Ascending(c => c.ParentId),
                new CreateIndexOptions { Name = "ParentCategoryIndex" });

            // Tạo index
            await db.Categories.Indexes.CreateOneAsync(parentIndex);
        }
    }
}
