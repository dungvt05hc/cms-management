// <copyright file="ReorderCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Orders.Reorder;

/// <summary>
/// Command to reorder items from an existing order.
/// </summary>
/// <param name="UserId">The user ID.</param>
/// <param name="OrderId">The order ID to reorder from.</param>
public record ReorderCommand(
    Guid UserId,
    Guid OrderId);
