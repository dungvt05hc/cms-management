// <copyright file="GetOrdersQuery.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Domain.Entities;

namespace Application.Features.Orders.GetOrders;

/// <summary>
/// Query to get user's orders with optional status filter.
/// </summary>
/// <param name="UserId">The authenticated user ID.</param>
/// <param name="Status">Optional status filter.</param>
public record GetOrdersQuery(Guid UserId, OrderStatus? Status);
