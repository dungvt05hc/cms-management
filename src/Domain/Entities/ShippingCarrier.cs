// <copyright file="ShippingCarrier.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Domain.Entities;

/// <summary>
/// Represents a shipping carrier (e.g., GHN, GHTK, VNPost).
/// </summary>
public class ShippingCarrier
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the shipping method ID.
    /// </summary>
    public Guid ShippingMethodId { get; set; }

    /// <summary>
    /// Gets or sets the carrier code (e.g., GHN, GHTK, VNPOST).
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether COD is supported.
    /// </summary>
    public bool SupportsCOD { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this carrier is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the created date.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last updated date.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the shipping method.
    /// </summary>
    public ShippingMethod ShippingMethod { get; set; } = null!;
}
