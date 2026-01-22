// <copyright file="OrderStatus.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Domain.Entities;

/// <summary>
/// Order status enumeration.
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Order is being processed.
    /// </summary>
    Processing = 0,

    /// <summary>
    /// Order is being shipped.
    /// </summary>
    Shipping = 1,

    /// <summary>
    /// Order has been delivered.
    /// </summary>
    Delivered = 2,

    /// <summary>
    /// Order has been cancelled.
    /// </summary>
    Cancelled = 3,
}
