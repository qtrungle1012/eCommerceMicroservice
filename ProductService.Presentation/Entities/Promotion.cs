namespace ProductService.Presentation.Entities
{
    public class Promotion
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AppliesToType { get; set; } = string.Empty; // "Product" hoặc "Category"
        public Guid TargetId { get; set; } // ID của sản phẩm hoặc danh mục
        public decimal Discount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
