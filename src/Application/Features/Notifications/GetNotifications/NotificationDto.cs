// <copyright file="NotificationDto.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Notifications.GetNotifications;

/// <summary>
/// Notification DTO.
/// </summary>
/// <param name="Id">Notification ID.</param>
/// <param name="Type">Notification type.</param>
/// <param name="Title">Notification title.</param>
/// <param name="Message">Notification message.</param>
/// <param name="Data">Additional data (JSON).</param>
/// <param name="IsRead">Whether the notification is read.</param>
/// <param name="CreatedAt">Created timestamp.</param>
public record NotificationDto(
    Guid Id,
    string Type,
    string Title,
    string Message,
    string? Data,
    bool IsRead,
    DateTime CreatedAt);
