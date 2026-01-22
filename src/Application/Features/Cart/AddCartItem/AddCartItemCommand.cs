// <copyright file="AddCartItemCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Cart.AddCartItem;

/// <summary>
/// Command to add an item to cart.
/// </summary>
/// <param name="UserId">The user ID.</param>
/// <param name="ProductId">The product ID.</param>
/// <param name="VariantId">The variant ID (optional).</param>
/// <param name="Quantity">The quantity.</param>
public record AddCartItemCommand(
    Guid UserId,
    Guid ProductId,
    Guid? VariantId,
    int Quantity);
