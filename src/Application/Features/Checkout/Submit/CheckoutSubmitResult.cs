// <copyright file="CheckoutSubmitResult.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Checkout.Submit;

/// <summary>
/// Result of checkout submit.
/// </summary>
public class CheckoutSubmitResult
{
    /// <summary>
    /// Gets or sets the order ID (only for COD, null for Payoo).
    /// </summary>
    public Guid? OrderId { get; set; }

    /// <summary>
    /// Gets or sets the payment URL (only for Payoo, null for COD).
    /// </summary>
    public string? PaymentUrl { get; set; }

    /// <summary>
    /// Gets or sets the payment reference (only for Payoo, null for COD).
    /// </summary>
    public string? PaymentReference { get; set; }
}
