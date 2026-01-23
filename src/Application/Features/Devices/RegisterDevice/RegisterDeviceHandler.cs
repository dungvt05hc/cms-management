// <copyright file="RegisterDeviceHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Devices.RegisterDevice;

/// <summary>
/// Handler for registering a device token.
/// </summary>
public class RegisterDeviceHandler
{
    private readonly IAppDbContext context;
    private readonly ILogger<RegisterDeviceHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterDeviceHandler"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger.</param>
    public RegisterDeviceHandler(
        IAppDbContext context,
        ILogger<RegisterDeviceHandler> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the register device command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Device ID.</returns>
    public async Task<Guid> Handle(
        RegisterDeviceCommand command,
        CancellationToken cancellationToken)
    {
        // Check if token already exists for this user (deduplication)
        var existingDevice = await this.context.DeviceTokens
            .FirstOrDefaultAsync(
                d => d.UserId == command.UserId && d.Token == command.Token,
                cancellationToken);

        if (existingDevice != null)
        {
            this.logger.LogInformation(
                "Device token already registered for user {UserId}",
                command.UserId);
            return existingDevice.Id;
        }

        var deviceToken = new DeviceToken
        {
            Id = Guid.NewGuid(),
            UserId = command.UserId,
            Token = command.Token,
            DeviceType = command.DeviceType,
            CreatedAt = DateTime.UtcNow,
        };

        this.context.DeviceTokens.Add(deviceToken);
        await this.context.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation(
            "Device token registered for user {UserId}",
            command.UserId);

        return deviceToken.Id;
    }
}
