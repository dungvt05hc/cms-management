// <copyright file="NotificationType.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Domain.Entities;

/// <summary>
/// Notification type enum.
/// </summary>
public enum NotificationType
{
    /// <summary>
    /// Promotion notification.
    /// </summary>
    Promotion,

    /// <summary>
    /// Order notification.
    /// </summary>
    Order,

    /// <summary>
    /// System notification.
    /// </summary>
    System,
}
