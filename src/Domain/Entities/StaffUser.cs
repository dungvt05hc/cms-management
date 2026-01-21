// <copyright file="StaffUser.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Domain.Entities;

/// <summary>
/// Represents a staff user in the admin/management system.
/// </summary>
public class StaffUser
{
    /// <summary>
    /// Gets or sets the unique identifier for the staff user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the email address (unique, required for staff).
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the phone number (optional).
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets the full name of the staff user.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password hash.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the role of the staff user.
    /// </summary>
    public StaffRole Role { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the staff account is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the date and time the staff user was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time the staff user was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
