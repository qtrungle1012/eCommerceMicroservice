﻿using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using SharedLibrarySolution.Responses;
using System.Text.Json;

namespace ProductService.Presentation.Features.Products.CreateProduct
{
    public static class CreateProductEndpoint
    {
        public static void MapCreateProductEndpoint(this IEndpointRouteBuilder routes)
        {
            routes.MapPost("/products", async (
                [FromForm] CreateProductRequest request,
                CreateProductHandler handler,
                IValidator<CreateProductRequest> validator  // validate
            ) =>
            {
                try
                {
                    // VALIDATE TRƯỚC KHI Thêm
                    var validationResult = await validator.ValidateAsync(request);
                    if (!validationResult.IsValid)
                    {
                        var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                        return Results.BadRequest(
                            new ApiResponse<ProductsResponse>(400, $"Validation failed: {errors}")
                        );
                    }

                    // GỌI HANDLER
                    var result = await handler.HandleAsync(request);
                    if (result == null)
                    {
                        return Results.BadRequest(
                            new ApiResponse<ProductsResponse>(400, "Failed to create product")
                        );
                    }

                    // 201 CREATED
                    return Results.Created(
                        $"/products/{result.Id}",
                        new ApiResponse<ProductsResponse>(201, "Product created successfully", result)
                    );
                }
                catch (JsonException ex)
                {
                    return Results.BadRequest(
                        new ApiResponse<ProductsResponse>(400, $"Invalid JSON data: {ex.Message}")
                    );
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(
                        new ApiResponse<ProductsResponse>(400, ex.Message)
                    );
                }
                catch (Exception ex)
                {
                    return Results.Json(
                        new ApiResponse<ProductsResponse>(500, $"Internal server error: {ex.Message}")
                    );
                }
            })
            .RequireAuthorization("RequireAdminRole", "RequireSellerRole")
            .WithTags("Products")
            .WithName("CreateProduct")
            .Accepts<CreateProductRequest>("multipart/form-data")
            .Produces<ApiResponse<ProductsResponse>>(StatusCodes.Status201Created)
            .Produces<ApiResponse<ProductsResponse>>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponse<ProductsResponse>>(StatusCodes.Status500InternalServerError)
            .DisableAntiforgery();
        }
    }
}