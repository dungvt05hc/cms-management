// <copyright file="ConfirmOrderCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Orders.ConfirmOrder;

/// <summary>
/// Command to confirm order received.
/// </summary>
/// <param name="UserId">The authenticated user ID.</param>
/// <param name="OrderId">The order ID.</param>
public record ConfirmOrderCommand(Guid UserId, Guid OrderId);
