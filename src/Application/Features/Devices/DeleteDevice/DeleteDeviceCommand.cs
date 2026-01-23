// <copyright file="DeleteDeviceCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Devices.DeleteDevice;

/// <summary>
/// Command to delete a device token.
/// </summary>
/// <param name="UserId">User ID.</param>
/// <param name="DeviceId">Device token ID to delete.</param>
public record DeleteDeviceCommand(
    Guid UserId,
    Guid DeviceId);
