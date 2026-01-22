// <copyright file="OrderItem.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Domain.Entities;

/// <summary>
/// Represents an item in an order.
/// </summary>
public class OrderItem
{
    /// <summary>
    /// Gets or sets the unique identifier for the order item.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the order identifier.
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// Gets or sets the order (navigation property).
    /// </summary>
    public Order? Order { get; set; }

    /// <summary>
    /// Gets or sets the product identifier.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the product (navigation property).
    /// </summary>
    public Product? Product { get; set; }

    /// <summary>
    /// Gets or sets the product variant identifier (optional).
    /// </summary>
    public Guid? VariantId { get; set; }

    /// <summary>
    /// Gets or sets the product variant (navigation property).
    /// </summary>
    public ProductVariant? Variant { get; set; }

    /// <summary>
    /// Gets or sets the product name (snapshot at time of order).
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the variant name (snapshot at time of order).
    /// </summary>
    public string? VariantName { get; set; }

    /// <summary>
    /// Gets or sets the SKU (snapshot at time of order).
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unit price at time of order.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the total price (UnitPrice * Quantity).
    /// </summary>
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// Gets or sets the date and time the order item was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time the order item was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
