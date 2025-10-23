using Microsoft.AspNetCore.Mvc;
using ProductService.Presentation.Features.Products.CreateProduct;
using SharedLibrarySolution.Responses;
using static MassTransit.ValidationResultExtensions;

namespace ProductService.Presentation.Features.Products.DeleteProduct
{
    public static class DeleteProductEndpoint
    {
        public static void MapDeleteProductEndpoint(this IEndpointRouteBuilder routes)
        {
            routes.MapDelete("/products/{id}", async (
                 [FromRoute] string id,
                 DeleteProductHandler handler
             ) =>
            {
                var response = await handler.HandleAsync(new DeleteProductRequest { Id = id }); // gọi handler
                if (!response)
                    return Results.NotFound($"Product with id {id} not found.");

                return Results.Ok($"Product {id} deleted successfully.");
            })
            .WithTags("Products")
            .Produces<ApiResponse<string>>(StatusCodes.Status200OK)
            .Produces<ApiResponse<string>>(StatusCodes.Status404NotFound)
            .DisableAntiforgery(); // để test Postman
        }
    }
}
