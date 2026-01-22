// <copyright file="SetDefaultAddressHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Addresses.SetDefaultAddress;

/// <summary>
/// Handler for SetDefaultAddressCommand.
/// </summary>
public class SetDefaultAddressHandler
{
    private readonly IAppDbContext dbContext;
    private readonly ILogger<SetDefaultAddressHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SetDefaultAddressHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger.</param>
    public SetDefaultAddressHandler(
        IAppDbContext dbContext,
        ILogger<SetDefaultAddressHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Handles setting an address as default.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated address.</returns>
    public async Task<AddressDto> Handle(SetDefaultAddressCommand command, CancellationToken cancellationToken)
    {
        var address = await this.dbContext.Addresses
            .FirstOrDefaultAsync(a => a.Id == command.AddressId && a.UserId == command.UserId, cancellationToken);

        if (address == null)
        {
            this.logger.LogWarning("Address {AddressId} not found for user {UserId}", command.AddressId, command.UserId);
            throw new InvalidOperationException("Address not found.");
        }

        var currentDefaults = await this.dbContext.Addresses
            .Where(a => a.UserId == command.UserId && a.IsDefault && a.Id != command.AddressId)
            .ToListAsync(cancellationToken);

        foreach (var addr in currentDefaults)
        {
            addr.IsDefault = false;
            addr.UpdatedAt = DateTime.UtcNow;
        }

        address.IsDefault = true;
        address.UpdatedAt = DateTime.UtcNow;

        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Set address {AddressId} as default for user {UserId}", command.AddressId, command.UserId);

        return new AddressDto(
            address.Id,
            address.UserId,
            address.FullName,
            address.Phone,
            address.AddressLine,
            address.Ward,
            address.District,
            address.City,
            address.IsDefault,
            address.CreatedAt,
            address.UpdatedAt);
    }
}
