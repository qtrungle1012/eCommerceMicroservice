using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using ProductService.Presentation.Entities;

namespace ProductService.Presentation.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private static bool _isConfigured = false;
        private static readonly object _lock = new object();

        public MongoDbContext(IConfiguration configuration)
        {
            // GỌI ConfigureGuidSerialization TRƯỚC KHI làm gì khác
            ConfigureGuidSerialization();

            var connectionString = configuration.GetConnectionString("MongoDb");
            var dbName = configuration["DatabaseSettings:DatabaseName"];

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(dbName);
            EnsureCollectionsExist();
        }

        // Cấu hình cách MongoDB serialize Guid (chỉ chạy 1 lần duy nhất)
        private static void ConfigureGuidSerialization()
        {
            if (_isConfigured) return;

            lock (_lock)
            {
                if (_isConfigured) return;

                // Cấu hình Guid serialization cho toàn bộ ứng dụng
                BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

                // Dùng Convention
                var pack = new ConventionPack
                {
                    new EnumRepresentationConvention(BsonType.String), // Enum lưu dạng string
                    new IgnoreExtraElementsConvention(true) // Bỏ qua field thừa
                };
                ConventionRegistry.Register("MyConventions", pack, t => true);

                _isConfigured = true;
            }
        }

        // Đảm bảo tạo db nếu chưa có
        private void EnsureCollectionsExist()
        {
            var collectionNames = _database.ListCollectionNames().ToList();

            if (!collectionNames.Contains("Products"))
                _database.CreateCollection("Products");
            if (!collectionNames.Contains("Categories"))
                _database.CreateCollection("Categories");
            if (!collectionNames.Contains("Promotions"))
                _database.CreateCollection("Promotions");
            if (!collectionNames.Contains("Reviews"))
                _database.CreateCollection("Reviews");
        }

        // Expose các collection như table trong SQL
        public IMongoCollection<Product> Products => _database.GetCollection<Product>("Products");
        public IMongoCollection<Category> Categories => _database.GetCollection<Category>("Categories");
        public IMongoCollection<Promotion> Promotions => _database.GetCollection<Promotion>("Promotions");
        public IMongoCollection<Review> Reviews => _database.GetCollection<Review>("Reviews");

        public IMongoDatabase Database => _database;
    }
}