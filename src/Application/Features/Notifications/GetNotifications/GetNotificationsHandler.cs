// <copyright file="GetNotificationsHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Notifications.GetNotifications;

/// <summary>
/// Handler for getting user notifications.
/// </summary>
public class GetNotificationsHandler
{
    private readonly IAppDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetNotificationsHandler"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public GetNotificationsHandler(IAppDbContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Handles the get notifications query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of notifications.</returns>
    public async Task<List<NotificationDto>> Handle(
        GetNotificationsQuery query,
        CancellationToken cancellationToken)
    {
        var notificationsQuery = this.context.Notifications
            .Where(n => n.UserId == query.UserId);

        // Apply type filter
        if (query.Type != "all")
        {
            // Map plural forms to singular enum values
            var typeToCheck = query.Type.TrimEnd('s');
            if (Enum.TryParse<NotificationType>(typeToCheck, true, out var notifType))
            {
                notificationsQuery = notificationsQuery.Where(n => n.Type == notifType);
            }
        }

        var notifications = await notificationsQuery
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationDto(
                n.Id,
                n.Type.ToString().ToLower(),
                n.Title,
                n.Message,
                n.Data,
                n.IsRead,
                n.CreatedAt))
            .ToListAsync(cancellationToken);

        return notifications;
    }
}
