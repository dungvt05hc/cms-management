// <copyright file="OrderItemDto.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Orders;

/// <summary>
/// DTO for order item information.
/// </summary>
public class OrderItemDto
{
    /// <summary>
    /// Gets or sets the order item ID.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the product ID.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the variant ID.
    /// </summary>
    public Guid? VariantId { get; set; }

    /// <summary>
    /// Gets or sets the product name.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the variant name.
    /// </summary>
    public string? VariantName { get; set; }

    /// <summary>
    /// Gets or sets the SKU.
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unit price.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the total price.
    /// </summary>
    public decimal TotalPrice { get; set; }
}
