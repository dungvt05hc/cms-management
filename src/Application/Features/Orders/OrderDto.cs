// <copyright file="OrderDto.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Domain.Entities;

namespace Application.Features.Orders;

/// <summary>
/// DTO for order information.
/// </summary>
public class OrderDto
{
    /// <summary>
    /// Gets or sets the order ID.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the order status.
    /// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the payment method.
    /// </summary>
    public PaymentMethod PaymentMethod { get; set; }

    /// <summary>
    /// Gets or sets the subtotal amount.
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// Gets or sets the discount amount.
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Gets or sets the shipping fee.
    /// </summary>
    public decimal ShippingFee { get; set; }

    /// <summary>
    /// Gets or sets the shipping discount.
    /// </summary>
    public decimal ShippingDiscount { get; set; }

    /// <summary>
    /// Gets or sets the total amount.
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Gets or sets the shipping full name.
    /// </summary>
    public string ShippingFullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the shipping phone.
    /// </summary>
    public string ShippingPhone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the shipping address line.
    /// </summary>
    public string ShippingAddressLine { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the shipping ward.
    /// </summary>
    public string ShippingWard { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the shipping district.
    /// </summary>
    public string ShippingDistrict { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the shipping city.
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
    /// Gets or sets the discount voucher code.
    /// </summary>
    public string? DiscountVoucherCode { get; set; }

    /// <summary>
    /// Gets or sets the shipping voucher code.
    /// </summary>
    public string? ShippingVoucherCode { get; set; }

    /// <summary>
    /// Gets or sets the buyer notes.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the order items.
    /// </summary>
    public List<OrderItemDto> Items { get; set; } = new();

    /// <summary>
    /// Gets or sets the created date.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the updated date.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
