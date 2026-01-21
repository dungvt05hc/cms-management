// <copyright file="Category.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Domain.Entities;

/// <summary>
/// Represents a product category with support for unlimited hierarchy.
/// </summary>
public class Category
{
    /// <summary>
    /// Gets or sets the unique identifier for the category.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the category name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the parent category identifier (null for root categories).
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Gets or sets the parent category (navigation property).
    /// </summary>
    public Category? Parent { get; set; }

    /// <summary>
    /// Gets or sets the child categories (navigation property).
    /// </summary>
    public ICollection<Category> Children { get; set; } = new List<Category>();

    /// <summary>
    /// Gets or sets the date and time the category was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time the category was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
