// <copyright file="User.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Domain.Entities;

/// <summary>
/// Represents a buyer/user in the e-commerce system.
/// </summary>
public class User
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the phone number (unique).
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address (optional, unique if provided).
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the full name of the user.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password hash.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the user account is verified (OTP confirmed).
    /// </summary>
    public bool IsVerified { get; set; }

    /// <summary>
    /// Gets or sets the date and time the user was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time the user was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
