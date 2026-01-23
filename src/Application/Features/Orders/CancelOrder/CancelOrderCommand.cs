// <copyright file="CancelOrderCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Orders.CancelOrder;

/// <summary>
/// Command to cancel an order.
/// </summary>
/// <param name="UserId">The authenticated user ID.</param>
/// <param name="OrderId">The order ID.</param>
/// <param name="ReasonCode">The cancellation reason code.</param>
/// <param name="Note">Optional cancellation note.</param>
public record CancelOrderCommand(Guid UserId, Guid OrderId, string ReasonCode, string? Note);
