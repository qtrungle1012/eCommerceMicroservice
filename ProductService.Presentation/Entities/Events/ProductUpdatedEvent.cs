namespace ProductService.Presentation.Entities.Events
{
    public class ProductUpdatedEvent
    {
        public string ProductId { get; set; } = default!;
        public string ProductName { get; set; } = default!;
        public string ImageUrl { get; set; } = default!; // CHỈ 1 ảnh đại diện
        public decimal Price { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
