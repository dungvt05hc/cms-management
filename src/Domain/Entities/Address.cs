// <copyright file="Address.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Domain.Entities;

/// <summary>
/// Represents a delivery/shipping address for a buyer.
/// </summary>
public class Address
{
    /// <summary>
    /// Gets or sets the unique identifier for the address.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user ID that owns this address.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the recipient full name.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the recipient phone number.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the address line (street, building, apartment).
    /// </summary>
    public string AddressLine { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ward/commune.
    /// </summary>
    public string Ward { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the district.
    /// </summary>
    public string District { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the city/province.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether this is the default address.
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Gets or sets the date and time the address was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time the address was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the user.
    /// </summary>
    public User? User { get; set; }
}
