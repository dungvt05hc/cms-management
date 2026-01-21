// <copyright file="ProductDto.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Products;

/// <summary>
/// Product data transfer object.
/// </summary>
/// <param name="Id">The product identifier.</param>
/// <param name="Name">The product name.</param>
/// <param name="Slug">The URL-friendly slug.</param>
/// <param name="Description">The product description.</param>
/// <param name="CategoryId">The category identifier.</param>
/// <param name="Images">The images.</param>
/// <param name="Videos">The videos.</param>
/// <param name="Specifications">The specifications.</param>
/// <param name="IsActive">Whether the product is active.</param>
/// <param name="Variants">The product variants.</param>
/// <param name="CreatedAt">The creation timestamp.</param>
/// <param name="UpdatedAt">The last update timestamp.</param>
public record ProductDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    Guid? CategoryId,
    string? Images,
    string? Videos,
    string? Specifications,
    bool IsActive,
    List<ProductVariantDto> Variants,
    DateTime CreatedAt,
    DateTime UpdatedAt);
