// <copyright file="DeviceToken.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Domain.Entities;

/// <summary>
/// Device token entity for FCM push notifications.
/// </summary>
public class DeviceToken
{
    /// <summary>
    /// Gets or sets the device token ID.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user ID.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the FCM token.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the device type (optional metadata).
    /// </summary>
    public string? DeviceType { get; set; }

    /// <summary>
    /// Gets or sets the created date.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to User.
    /// </summary>
    public User User { get; set; } = null!;
}
