// <copyright file="GetDevicesHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Devices.GetDevices;

/// <summary>
/// Handler for getting user device tokens.
/// </summary>
public class GetDevicesHandler
{
    private readonly IAppDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetDevicesHandler"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public GetDevicesHandler(IAppDbContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Handles the get devices query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of device tokens.</returns>
    public async Task<List<DeviceTokenDto>> Handle(
        GetDevicesQuery query,
        CancellationToken cancellationToken)
    {
        var devices = await this.context.DeviceTokens
            .Where(d => d.UserId == query.UserId)
            .OrderByDescending(d => d.CreatedAt)
            .Select(d => new DeviceTokenDto(
                d.Id,
                MaskToken(d.Token),
                d.DeviceType,
                d.CreatedAt))
            .ToListAsync(cancellationToken);

        return devices;
    }

    private static string MaskToken(string token)
    {
        if (string.IsNullOrEmpty(token) || token.Length <= 8)
        {
            return "****";
        }

        return $"{token.Substring(0, 4)}...{token.Substring(token.Length - 4)}";
    }
}
