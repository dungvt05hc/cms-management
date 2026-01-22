// <copyright file="CheckoutTotalsDto.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Checkout;

/// <summary>
/// DTO for checkout totals including discount and shipping vouchers.
/// </summary>
public class CheckoutTotalsDto
{
    /// <summary>
    /// Gets or sets the cart subtotal before discounts.
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// Gets or sets the discount amount from discount voucher.
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Gets or sets the shipping fee before voucher.
    /// </summary>
    public decimal ShippingFee { get; set; }

    /// <summary>
    /// Gets or sets the shipping discount amount from shipping voucher.
    /// </summary>
    public decimal ShippingDiscount { get; set; }

    /// <summary>
    /// Gets or sets the final total amount.
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Gets or sets the applied discount voucher code (if any).
    /// </summary>
    public string? DiscountVoucherCode { get; set; }

    /// <summary>
    /// Gets or sets the applied shipping voucher code (if any).
    /// </summary>
    public string? ShippingVoucherCode { get; set; }
}
