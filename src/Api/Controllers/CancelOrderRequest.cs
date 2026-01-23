// <copyright file="CancelOrderRequest.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Api.Controllers;

/// <summary>
/// Request model for canceling an order.
/// </summary>
public class CancelOrderRequest
{
    /// <summary>
    /// Gets or sets the cancellation reason code.
    /// </summary>
    public string ReasonCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional cancellation note.
    /// </summary>
    public string? Note { get; set; }
}
