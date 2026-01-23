// <copyright file="DevicesController.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Security.Claims;

using Application.Features.Devices.DeleteDevice;
using Application.Features.Devices.GetDevices;
using Application.Features.Devices.RegisterDevice;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Device tokens controller for authenticated buyers.
/// </summary>
[ApiController]
[Route("me/devices")]
[Authorize]
public class DevicesController : ControllerBase
{
    private readonly GetDevicesHandler getDevicesHandler;
    private readonly RegisterDeviceHandler registerDeviceHandler;
    private readonly DeleteDeviceHandler deleteDeviceHandler;
    private readonly IValidator<RegisterDeviceCommand> registerDeviceValidator;
    private readonly ILogger<DevicesController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DevicesController"/> class.
    /// </summary>
    /// <param name="getDevicesHandler">The get devices handler.</param>
    /// <param name="registerDeviceHandler">The register device handler.</param>
    /// <param name="deleteDeviceHandler">The delete device handler.</param>
    /// <param name="registerDeviceValidator">The register device validator.</param>
    /// <param name="logger">The logger.</param>
    public DevicesController(
        GetDevicesHandler getDevicesHandler,
        RegisterDeviceHandler registerDeviceHandler,
        DeleteDeviceHandler deleteDeviceHandler,
        IValidator<RegisterDeviceCommand> registerDeviceValidator,
        ILogger<DevicesController> logger)
    {
        this.getDevicesHandler = getDevicesHandler;
        this.registerDeviceHandler = registerDeviceHandler;
        this.deleteDeviceHandler = deleteDeviceHandler;
        this.registerDeviceValidator = registerDeviceValidator;
        this.logger = logger;
    }

    /// <summary>
    /// Get all device tokens for the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of device tokens.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<DeviceTokenDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDevices(CancellationToken cancellationToken)
    {
        var userId = this.GetUserId();
        if (!userId.HasValue)
        {
            return this.Unauthorized(new { Message = "Invalid token." });
        }

        var query = new GetDevicesQuery(userId.Value);
        var result = await this.getDevicesHandler.Handle(query, cancellationToken);

        return this.Ok(result);
    }

    /// <summary>
    /// Register a new device token.
    /// </summary>
    /// <param name="request">The register device request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The device ID.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RegisterDevice(
        [FromBody] RegisterDeviceRequest request,
        CancellationToken cancellationToken)
    {
        var userId = this.GetUserId();
        if (!userId.HasValue)
        {
            return this.Unauthorized(new { Message = "Invalid token." });
        }

        var command = new RegisterDeviceCommand(
            userId.Value,
            request.Token,
            request.DeviceType);

        var validationResult = await this.registerDeviceValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return this.BadRequest(new
            {
                Message = "Validation failed.",
                Errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }),
            });
        }

        var deviceId = await this.registerDeviceHandler.Handle(command, cancellationToken);

        return this.CreatedAtAction(nameof(this.GetDevices), null, new { Id = deviceId });
    }

    /// <summary>
    /// Delete a device token.
    /// </summary>
    /// <param name="id">Device ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDevice(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = this.GetUserId();
        if (!userId.HasValue)
        {
            return this.Unauthorized(new { Message = "Invalid token." });
        }

        try
        {
            var command = new DeleteDeviceCommand(userId.Value, id);
            await this.deleteDeviceHandler.Handle(command, cancellationToken);
            return this.NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return this.NotFound(new { Message = ex.Message });
        }
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
