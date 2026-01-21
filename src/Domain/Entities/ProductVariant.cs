// <copyright file="ProductVariant.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Domain.Entities;

/// <summary>
/// Represents a product variant with price and stock.
/// </summary>
public class ProductVariant
{
    /// <summary>
    /// Gets or sets the unique identifier for the variant.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the product identifier.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the product (navigation property).
    /// </summary>
    public Product Product { get; set; } = null!;

    /// <summary>
    /// Gets or sets the SKU (Stock Keeping Unit) - must be unique across all variants.
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the variant name or attributes (e.g., "Red - Large").
    /// </summary>
    public string? VariantName { get; set; }

    /// <summary>
    /// Gets or sets the price.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the stock quantity.
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Gets or sets the date and time the variant was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time the variant was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
