// <copyright file="UpdateProductVariantDto.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Products.UpdateProduct;

/// <summary>
/// Product variant for update.
/// </summary>
/// <param name="Id">The variant identifier (null for new variants).</param>
/// <param name="Sku">The SKU (must be unique).</param>
/// <param name="VariantName">The variant name or attributes.</param>
/// <param name="Price">The price.</param>
/// <param name="StockQuantity">The stock quantity.</param>
public record UpdateProductVariantDto(
    Guid? Id,
    string Sku,
    string? VariantName,
    decimal Price,
    int StockQuantity);
