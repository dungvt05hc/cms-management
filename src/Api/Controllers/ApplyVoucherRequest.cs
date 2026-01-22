// <copyright file="ApplyVoucherRequest.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Api.Controllers;

/// <summary>
/// Request model for applying vouchers.
/// </summary>
public class ApplyVoucherRequest
{
    /// <summary>
    /// Gets or sets the discount voucher code (optional).
    /// </summary>
    public string? DiscountCode { get; set; }

    /// <summary>
    /// Gets or sets the shipping voucher code (optional).
    /// </summary>
    public string? ShippingCode { get; set; }
}
