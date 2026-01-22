// <copyright file="Order.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Domain.Entities;

/// <summary>
/// Represents an order placed by a user.
/// </summary>
public class Order
{
    /// <summary>
    /// Gets or sets the unique identifier for the order.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the user (navigation property).
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// Gets or sets the order items.
    /// </summary>
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    /// <summary>
    /// Gets or sets the subtotal amount (before discounts).
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// Gets or sets the discount amount from voucher.
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Gets or sets the shipping fee.
    /// </summary>
    public decimal ShippingFee { get; set; }

    /// <summary>
    /// Gets or sets the shipping discount amount from shipping voucher.
    /// </summary>
    public decimal ShippingDiscount { get; set; }

    /// <summary>
    /// Gets or sets the total amount.
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Gets or sets the payment method.
    /// </summary>
    public PaymentMethod PaymentMethod { get; set; }

    /// <summary>
    /// Gets or sets the order status.
    /// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the shipping address full name.
    /// </summary>
    public string ShippingFullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the shipping address phone.
    /// </summary>
    public string ShippingPhone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the shipping address line.
    /// </summary>
    public string ShippingAddressLine { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the shipping address ward.
    /// </summary>
    public string ShippingWard { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the shipping address district.
    /// </summary>
    public string ShippingDistrict { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the shipping address city.
    /// </summary>
    public string ShippingCity { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the shipping method code.
    /// </summary>
    public string ShippingMethodCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the shipping carrier code.
    /// </summary>
    public string ShippingCarrierCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the discount voucher code (if applied).
    /// </summary>
    public string? DiscountVoucherCode { get; set; }

    /// <summary>
    /// Gets or sets the shipping voucher code (if applied).
    /// </summary>
    public string? ShippingVoucherCode { get; set; }

    /// <summary>
    /// Gets or sets the buyer notes.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the payment reference (for Payoo).
    /// </summary>
    public string? PaymentReference { get; set; }

    /// <summary>
    /// Gets or sets the date and time the order was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time the order was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
