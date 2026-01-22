// <copyright file="DeleteCartItemCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Cart.DeleteCartItem;

/// <summary>
/// Command to delete a cart item.
/// </summary>
/// <param name="UserId">The user ID.</param>
/// <param name="ItemId">The cart item ID.</param>
public record DeleteCartItemCommand(Guid UserId, Guid ItemId);
