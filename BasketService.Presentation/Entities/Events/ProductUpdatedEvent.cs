namespace BasketService.Presentation.Entities.Events
{
    public class ProductUpdatedEvent 
    {
        public string ProductId { get; set; } = default!;
        public string ProductName { get; set; } = default!;
        public string ImageUrl { get; set; } = default!;
        public decimal Price { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
