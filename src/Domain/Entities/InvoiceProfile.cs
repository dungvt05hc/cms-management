// <copyright file="InvoiceProfile.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Domain.Entities;

/// <summary>
/// Represents a VAT invoice profile for a buyer.
/// </summary>
public class InvoiceProfile
{
    /// <summary>
    /// Gets or sets the unique identifier for the invoice profile.
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
    /// Gets or sets the tax code (MST).
    /// </summary>
    public string TaxCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the company name.
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the company address.
    /// </summary>
    public string CompanyAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the invoice email.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time the profile was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time the profile was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
