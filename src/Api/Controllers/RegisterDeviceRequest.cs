// <copyright file="RegisterDeviceRequest.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Api.Controllers;

/// <summary>
/// Request to register a device token.
/// </summary>
/// <param name="Token">FCM device token.</param>
/// <param name="DeviceType">Device type (optional).</param>
public record RegisterDeviceRequest(
    string Token,
    string? DeviceType = null);
