namespace ProductService.Presentation.Entities
{
    public class Product
    {
        public Guid Id { get; set; } // ObjectId trong MongoDB
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; } // Tham chiếu tới Categories
        public List<ProductVariant>? Variants { get; set; } // Nhúng biến thể
        public List<string>? ImageUrls { get; set; } // Nhúng danh sách ảnh
        public double RatingAverage { get; set; } // Tính từ Reviews
        public bool IsDeleted { get; set; } = false; // xóa thì đánh dấu xóa đảm bảo tính soft delete
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    // là một biến thể của sản phẩm VD: {áo đen- giá 100 - số lượng: 20}, {áo trắng- giá 150 - số lượng: 10}
    public class ProductVariant
    {
        public string Sku { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
}
