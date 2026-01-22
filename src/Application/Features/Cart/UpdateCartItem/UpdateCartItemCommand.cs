// <copyright file="UpdateCartItemCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Cart.UpdateCartItem;

/// <summary>
/// Command to update a cart item.
/// </summary>
/// <param name="UserId">The user ID.</param>
/// <param name="ItemId">The cart item ID.</param>
/// <param name="Quantity">The new quantity (optional).</param>
/// <param name="VariantId">The new variant ID (optional).</param>
/// <param name="Selected">The new selected state (optional).</param>
public record UpdateCartItemCommand(
    Guid UserId,
    Guid ItemId,
    int? Quantity,
    Guid? VariantId,
    bool? Selected);
