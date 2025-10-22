using ProductService.Presentation.Common.Mapping;
using ProductService.Presentation.Entities;

namespace ProductService.Presentation.Features.Products
{
    public class ProductVariantResponse : IMapFrom<ProductVariant>
    {
        public string Sku { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    public class ProductsResponse : IMapFrom<Product>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }
        public double RatingAverage { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public List<ProductVariantResponse> Variants { get; set; } = new(); 
    }
}
