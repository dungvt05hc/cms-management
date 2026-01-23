// <copyright file="ProductGroupAttribute.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Domain.Entities;

/// <summary>
/// Represents an attribute definition for a product group.
/// </summary>
public class ProductGroupAttribute
{
    /// <summary>
    /// Gets or sets the unique identifier for the attribute.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the product group identifier.
    /// </summary>
    public Guid ProductGroupId { get; set; }

    /// <summary>
    /// Gets or sets the product group (navigation property).
    /// </summary>
    public ProductGroup? ProductGroup { get; set; }

    /// <summary>
    /// Gets or sets the attribute name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the attribute key (unique within the group).
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the attribute type (e.g., text, number, select).
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time the attribute was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time the attribute was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
