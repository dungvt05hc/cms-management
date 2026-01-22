// <copyright file="AddCartItemRequest.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Api.Controllers;

/// <summary>
/// Request model for adding a cart item.
/// </summary>
/// <param name="ProductId">The product ID.</param>
/// <param name="VariantId">The variant ID (optional).</param>
/// <param name="Quantity">The quantity.</param>
public record AddCartItemRequest(Guid ProductId, Guid? VariantId, int Quantity);
