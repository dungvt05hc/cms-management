// <copyright file="Cart.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Domain.Entities;

/// <summary>
/// Represents a shopping cart for a user.
/// </summary>
public class Cart
{
    /// <summary>
    /// Gets or sets the unique identifier for the cart.
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
    /// Gets or sets the cart items.
    /// </summary>
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();

    /// <summary>
    /// Gets or sets the date and time the cart was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time the cart was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
