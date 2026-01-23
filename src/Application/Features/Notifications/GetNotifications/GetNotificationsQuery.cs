// <copyright file="GetNotificationsQuery.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Notifications.GetNotifications;

/// <summary>
/// Query to get notifications for a user.
/// </summary>
/// <param name="UserId">User ID.</param>
/// <param name="Type">Notification type filter (all, promotions, orders, system). Default is "all".</param>
public record GetNotificationsQuery(
    Guid UserId,
    string Type = "all");
