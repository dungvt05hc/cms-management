// <copyright file="RegisterDeviceCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Devices.RegisterDevice;

/// <summary>
/// Command to register a device token.
/// </summary>
/// <param name="UserId">User ID.</param>
/// <param name="Token">FCM device token.</param>
/// <param name="DeviceType">Device type (optional).</param>
public record RegisterDeviceCommand(
    Guid UserId,
    string Token,
    string? DeviceType = null);
