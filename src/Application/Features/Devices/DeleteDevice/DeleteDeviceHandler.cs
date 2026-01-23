// <copyright file="DeleteDeviceHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Devices.DeleteDevice;

/// <summary>
/// Handler for deleting a device token.
/// </summary>
public class DeleteDeviceHandler
{
    private readonly IAppDbContext context;
    private readonly ILogger<DeleteDeviceHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteDeviceHandler"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger.</param>
    public DeleteDeviceHandler(
        IAppDbContext context,
        ILogger<DeleteDeviceHandler> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the delete device command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the operation.</returns>
    public async Task Handle(
        DeleteDeviceCommand command,
        CancellationToken cancellationToken)
    {
        var device = await this.context.DeviceTokens
            .FirstOrDefaultAsync(
                d => d.Id == command.DeviceId && d.UserId == command.UserId,
                cancellationToken);

        if (device == null)
        {
            throw new InvalidOperationException("Device token not found.");
        }

        this.context.DeviceTokens.Remove(device);
        await this.context.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation(
            "Device token {DeviceId} deleted for user {UserId}",
            command.DeviceId,
            command.UserId);
    }
}
