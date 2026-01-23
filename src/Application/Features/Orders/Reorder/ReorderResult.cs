// <copyright file="ReorderResult.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Orders.Reorder;

/// <summary>
/// Result of reorder operation.
/// </summary>
/// <param name="CartId">The cart ID where items were added.</param>
public record ReorderResult(Guid CartId);
