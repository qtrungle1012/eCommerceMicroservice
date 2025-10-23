using AutoMapper;
using MongoDB.Driver;
using Newtonsoft.Json;
using ProductService.Presentation.Data;
using ProductService.Presentation.Entities;
using ProductService.Presentation.Features.Products.CreateProduct;
using ProductService.Presentation.Services;

namespace ProductService.Presentation.Features.Products.UpdateProduct
{
    public class UpdateProductHandler
    {
        private readonly MongoDbContext _context;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IMapper _mapper;

        public UpdateProductHandler(MongoDbContext context, CloudinaryService cloudinaryService, IMapper mapper)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
            _mapper = mapper;
        }

        public async Task<ProductsResponse?> HandleAsync(string id, UpdateProductRequest request)
        {
            var product = await _context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (product == null)
                throw new ArgumentException("Product not found");

            // Parse variants nếu có gửi
            List<ProductVariantRequestDto>? variantDtos = null;
            if (!string.IsNullOrEmpty(request.VariantsJson))
            {
                variantDtos = JsonConvert.DeserializeObject<List<ProductVariantRequestDto>>(request.VariantsJson);
            }

            // Xử lý ảnh
            List<string> imageUrls = product.ImageUrls?.ToList() ?? new List<string>();

            // parse chuỗi danh sách ảnh cũ về dạng json
            if (!string.IsNullOrEmpty(request.OldImageUrls))
            {
                try
                {
                    var oldUrls = JsonConvert.DeserializeObject<List<string>>(request.OldImageUrls)
                        ?.Where(url => !string.IsNullOrWhiteSpace(url))
                        .ToList();

                    if (oldUrls != null)
                        imageUrls = oldUrls;
                }
                catch
                {
                    throw new ArgumentException("Invalid OldImageUrls JSON format");
                }
            }

            // Upload ảnh mới (nếu có)
            if (request.Images != null && request.Images.Any())
            {
                foreach (var img in request.Images)
                {
                    var imgUrl = await _cloudinaryService.UploadFileAsync(img);
                    imageUrls.Add(imgUrl);
                }
            }

            // Cập nhật dữ liệu (chỉ cập nhật nếu có gửi)
            if (!string.IsNullOrWhiteSpace(request.Name))
                product.Name = request.Name;

            if (!string.IsNullOrWhiteSpace(request.Description))
                product.Description = request.Description;

            if (request.Price != 0)
                product.Price = request.Price;

            if (!string.IsNullOrWhiteSpace(request.CategoryId))
                product.CategoryId = request.CategoryId;

            if (variantDtos != null)
            {
                product.Variants = variantDtos.Select(v => new ProductVariant
                {
                    Sku = v.Sku,
                    Price = v.Price,
                    StockQuantity = v.StockQuantity
                }).ToList();
            }

            // Cập nhật ảnh cuối cùng
            product.ImageUrls = imageUrls;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.Products.ReplaceOneAsync(p => p.Id == id, product);
            return _mapper.Map<ProductsResponse>(product);
        }
    }
}
