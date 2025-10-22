using SharedLibrarySolution.Responses;

namespace ProductService.Presentation.Features.Products.GetProducts
{
    public static class GetProductsEndpoint
    {
        public static void MapGetProductsEndpoint(this IEndpointRouteBuilder routes)
        {
            routes.MapGet("/products", async (
                [AsParameters] GetProductsQuery query,
                GetProductsHandler handler) =>
            {
                var response = await handler.HandleAsync(query);

                return Results.Ok(response);
            })
            .WithTags("Products")
            .WithName("GetProducts")

            .Produces<ApiResponse<PageResponse<ProductsResponse>>>(StatusCodes.Status200OK);
        }
    }
}