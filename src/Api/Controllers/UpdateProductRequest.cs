// <copyright file="UpdateProductRequest.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Features.Products.UpdateProduct;

namespace Api.Controllers;

/// <summary>
/// Update product request.
/// </summary>
/// <param name="Name">The product name.</param>
/// <param name="Slug">The URL-friendly slug.</param>
/// <param name="Description">The product description.</param>
/// <param name="CategoryId">The category identifier.</param>
/// <param name="Images">The images.</param>
/// <param name="Videos">The videos.</param>
/// <param name="Specifications">The specifications.</param>
/// <param name="IsActive">Whether the product is active.</param>
/// <param name="Variants">The product variants.</param>
public record UpdateProductRequest(
    string Name,
    string Slug,
    string? Description,
    Guid? CategoryId,
    string? Images,
    string? Videos,
    string? Specifications,
    bool IsActive,
    List<UpdateProductVariantDto> Variants);
