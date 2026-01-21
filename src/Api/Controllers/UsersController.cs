// <copyright file="UsersController.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Security.Claims;

using Application.Features.Users.GetProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Users controller.
/// </summary>
[ApiController]
[Route("me")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly GetProfileHandler getProfileHandler;
    private readonly ILogger<UsersController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersController"/> class.
    /// </summary>
    /// <param name="getProfileHandler">The get profile handler.</param>
    /// <param name="logger">The logger.</param>
    public UsersController(
        GetProfileHandler getProfileHandler,
        ILogger<UsersController> logger)
    {
        this.getProfileHandler = getProfileHandler;
        this.logger = logger;
    }

    /// <summary>
    /// Get authenticated user's profile.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user profile.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        var userIdClaim = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return this.Unauthorized(new { Message = "Invalid token." });
        }

        try
        {
            var query = new GetProfileQuery(userId);
            var result = await this.getProfileHandler.Handle(query, cancellationToken);
            return this.Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return this.NotFound(new { Message = ex.Message });
        }
    }
}
