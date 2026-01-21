// <copyright file="UpdateProductCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Products.UpdateProduct;

/// <summary>
/// Command to update a product.
/// </summary>
/// <param name="ProductId">The product identifier.</param>
/// <param name="Name">The product name.</param>
/// <param name="Slug">The URL-friendly slug (must be unique).</param>
/// <param name="Description">The product description.</param>
/// <param name="CategoryId">The category identifier.</param>
/// <param name="Images">The images.</param>
/// <param name="Videos">The videos.</param>
/// <param name="Specifications">The specifications.</param>
/// <param name="IsActive">Whether the product is active.</param>
/// <param name="IsFeatured">Whether the product is featured on the home page.</param>
/// <param name="Variants">The product variants.</param>
public record UpdateProductCommand(
    Guid ProductId,
    string Name,
    string Slug,
    string? Description,
    Guid? CategoryId,
    string? Images,
    string? Videos,
    string? Specifications,
    bool IsActive,
    bool IsFeatured,
    List<UpdateProductVariantDto> Variants);
