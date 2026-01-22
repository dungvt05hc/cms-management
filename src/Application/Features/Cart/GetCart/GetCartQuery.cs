// <copyright file="GetCartQuery.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Cart.GetCart;

/// <summary>
/// Query to get cart for a user.
/// </summary>
/// <param name="UserId">The user ID.</param>
public record GetCartQuery(Guid UserId);
