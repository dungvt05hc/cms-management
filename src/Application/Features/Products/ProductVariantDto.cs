// <copyright file="ProductVariantDto.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Products;

/// <summary>
/// Product variant data transfer object.
/// </summary>
/// <param name="Id">The variant identifier.</param>
/// <param name="Sku">The SKU.</param>
/// <param name="VariantName">The variant name or attributes.</param>
/// <param name="Price">The price.</param>
/// <param name="StockQuantity">The stock quantity.</param>
public record ProductVariantDto(
    Guid Id,
    string Sku,
    string? VariantName,
    decimal Price,
    int StockQuantity);
