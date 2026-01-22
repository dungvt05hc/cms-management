// <copyright file="UpdateCartItemRequest.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Api.Controllers;

/// <summary>
/// Request model for updating a cart item.
/// </summary>
/// <param name="Quantity">The new quantity (optional).</param>
/// <param name="VariantId">The new variant ID (optional).</param>
/// <param name="Selected">The new selected state (optional).</param>
public record UpdateCartItemRequest(int? Quantity, Guid? VariantId, bool? Selected);
