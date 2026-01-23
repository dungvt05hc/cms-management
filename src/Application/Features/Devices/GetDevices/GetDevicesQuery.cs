// <copyright file="GetDevicesQuery.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Devices.GetDevices;

/// <summary>
/// Query to get device tokens for a user.
/// </summary>
/// <param name="UserId">User ID.</param>
public record GetDevicesQuery(Guid UserId);
