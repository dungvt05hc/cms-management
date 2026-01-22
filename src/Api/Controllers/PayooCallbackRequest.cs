// <copyright file="PayooCallbackRequest.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Api.Controllers;

/// <summary>
/// Request model for Payoo payment callback.
/// </summary>
public class PayooCallbackRequest
{
    /// <summary>
    /// Gets or sets the payment reference.
    /// </summary>
    public string PaymentReference { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user ID.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the shipping address ID.
    /// </summary>
    public Guid AddressId { get; set; }

    /// <summary>
    /// Gets or sets the shipping method code.
    /// </summary>
    public string ShippingMethodCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the shipping carrier code.
    /// </summary>
    public string ShippingCarrierCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the discount voucher code (optional).
    /// </summary>
    public string? DiscountCode { get; set; }

    /// <summary>
    /// Gets or sets the shipping voucher code (optional).
    /// </summary>
    public string? ShippingCode { get; set; }

    /// <summary>
    /// Gets or sets the buyer notes (optional).
    /// </summary>
    public string? Notes { get; set; }
}
