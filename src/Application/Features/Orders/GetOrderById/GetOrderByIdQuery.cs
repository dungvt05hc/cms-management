// <copyright file="GetOrderByIdQuery.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Orders.GetOrderById;

/// <summary>
/// Query to get order by ID.
/// </summary>
/// <param name="UserId">The authenticated user ID.</param>
/// <param name="OrderId">The order ID.</param>
public record GetOrderByIdQuery(Guid UserId, Guid OrderId);
