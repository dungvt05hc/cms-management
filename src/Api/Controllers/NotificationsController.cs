// <copyright file="NotificationsController.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Security.Claims;

using Application.Features.Notifications.GetNotifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Notifications controller for authenticated buyers.
/// </summary>
[ApiController]
[Route("me/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly GetNotificationsHandler getNotificationsHandler;
    private readonly ILogger<NotificationsController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationsController"/> class.
    /// </summary>
    /// <param name="getNotificationsHandler">The get notifications handler.</param>
    /// <param name="logger">The logger.</param>
    public NotificationsController(
        GetNotificationsHandler getNotificationsHandler,
        ILogger<NotificationsController> logger)
    {
        this.getNotificationsHandler = getNotificationsHandler;
        this.logger = logger;
    }

    /// <summary>
    /// Get notifications for the authenticated user.
    /// </summary>
    /// <param name="type">Notification type filter: all, promotions, orders, system.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of notifications.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<NotificationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetNotifications(
        [FromQuery] string type = "all",
        CancellationToken cancellationToken = default)
    {
        var userId = this.GetUserId();
        if (!userId.HasValue)
        {
            return this.Unauthorized(new { Message = "Invalid token." });
        }

        var query = new GetNotificationsQuery(userId.Value, type);
        var result = await this.getNotificationsHandler.Handle(query, cancellationToken);

        return this.Ok(result);
    }

    private Guid? GetUserId()
    {
        var userIdClaim = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return null;
        }

        return userId;
    }
}
