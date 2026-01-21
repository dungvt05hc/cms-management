// <copyright file="CreateProductVariantDto.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Products.CreateProduct;

/// <summary>
/// Product variant for creation.
/// </summary>
/// <param name="Sku">The SKU (must be unique).</param>
/// <param name="VariantName">The variant name or attributes.</param>
/// <param name="Price">The price.</param>
/// <param name="StockQuantity">The stock quantity.</param>
public record CreateProductVariantDto(
    string Sku,
    string? VariantName,
    decimal Price,
    int StockQuantity);
