// <copyright file="CartItem.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Domain.Entities;

/// <summary>
/// Represents an item in a shopping cart.
/// </summary>
public class CartItem
{
    /// <summary>
    /// Gets or sets the unique identifier for the cart item.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the cart identifier.
    /// </summary>
    public Guid CartId { get; set; }

    /// <summary>
    /// Gets or sets the cart (navigation property).
    /// </summary>
    public Cart? Cart { get; set; }

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
    /// Gets or sets the quantity.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this item is selected for checkout.
    /// </summary>
    public bool Selected { get; set; }

    /// <summary>
    /// Gets or sets the date and time the cart item was added.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time the cart item was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
