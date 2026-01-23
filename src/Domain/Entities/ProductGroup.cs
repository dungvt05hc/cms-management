// <copyright file="ProductGroup.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Domain.Entities;

/// <summary>
/// Represents a product group linked to a category with attribute definitions.
/// </summary>
public class ProductGroup
{
    /// <summary>
    /// Gets or sets the unique identifier for the product group.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the category identifier.
    /// </summary>
    public Guid CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the category (navigation property).
    /// </summary>
    public Category? Category { get; set; }

    /// <summary>
    /// Gets or sets the product group name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product group attributes.
    /// </summary>
    public ICollection<ProductGroupAttribute> Attributes { get; set; } = new List<ProductGroupAttribute>();

    /// <summary>
    /// Gets or sets the date and time the product group was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time the product group was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
