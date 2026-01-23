// <copyright file="DeviceTokenDto.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Devices.GetDevices;

/// <summary>
/// Device token DTO.
/// </summary>
/// <param name="Id">Device token ID.</param>
/// <param name="Token">FCM token (masked for display).</param>
/// <param name="DeviceType">Device type.</param>
/// <param name="CreatedAt">Created timestamp.</param>
public record DeviceTokenDto(
    Guid Id,
    string Token,
    string? DeviceType,
    DateTime CreatedAt);
