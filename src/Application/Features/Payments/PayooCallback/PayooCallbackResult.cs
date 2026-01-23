// <copyright file="PayooCallbackResult.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Payments.PayooCallback;

/// <summary>
/// Result of Payoo callback processing.
/// </summary>
public class PayooCallbackResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the callback was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the order ID (if order was created).
    /// </summary>
    public Guid? OrderId { get; set; }

    /// <summary>
    /// Gets or sets the error message (if failed).
    /// </summary>
    public string? ErrorMessage { get; set; }
}
