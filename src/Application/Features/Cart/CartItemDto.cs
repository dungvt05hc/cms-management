// <copyright file="CartItemDto.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Cart;

/// <summary>
/// Cart item data transfer object.
/// </summary>
/// <param name="Id">Cart item ID.</param>
/// <param name="ProductId">Product ID.</param>
/// <param name="ProductName">Product name.</param>
/// <param name="ProductSlug">Product slug.</param>
/// <param name="VariantId">Variant ID (optional).</param>
/// <param name="VariantName">Variant name (optional).</param>
/// <param name="Price">Unit price.</param>
/// <param name="Quantity">Quantity.</param>
/// <param name="Selected">Whether item is selected for checkout.</param>
/// <param name="LineTotal">Line total (price * quantity).</param>
public record CartItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string ProductSlug,
    Guid? VariantId,
    string? VariantName,
    decimal Price,
    int Quantity,
    bool Selected,
    decimal LineTotal);
