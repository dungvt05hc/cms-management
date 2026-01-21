// <copyright file="Product.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Domain.Entities;

/// <summary>
/// Represents a product in the catalog.
/// </summary>
public class Product
{
    /// <summary>
    /// Gets or sets the unique identifier for the product.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the product name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL-friendly slug (must be unique).
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the category identifier.
    /// </summary>
    public Guid? CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the category (navigation property).
    /// </summary>
    public Category? Category { get; set; }

    /// <summary>
    /// Gets or sets the images (stored as URLs or media IDs).
    /// </summary>
    public string? Images { get; set; }

    /// <summary>
    /// Gets or sets the videos (stored as URLs or media IDs).
    /// </summary>
    public string? Videos { get; set; }

    /// <summary>
    /// Gets or sets the specifications (JSON or text).
    /// </summary>
    public string? Specifications { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the product is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the product variants.
    /// </summary>
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();

    /// <summary>
    /// Gets or sets the date and time the product was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time the product was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
