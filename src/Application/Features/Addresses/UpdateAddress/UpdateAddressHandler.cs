// <copyright file="UpdateAddressHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Addresses.UpdateAddress;

/// <summary>
/// Handler for UpdateAddressCommand.
/// </summary>
public class UpdateAddressHandler
{
    private readonly IAppDbContext dbContext;
    private readonly ILogger<UpdateAddressHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateAddressHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger.</param>
    public UpdateAddressHandler(
        IAppDbContext dbContext,
        ILogger<UpdateAddressHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Handles updating an address.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated address.</returns>
    public async Task<AddressDto> Handle(UpdateAddressCommand command, CancellationToken cancellationToken)
    {
        var address = await this.dbContext.Addresses
            .FirstOrDefaultAsync(a => a.Id == command.AddressId && a.UserId == command.UserId, cancellationToken);

        if (address == null)
        {
            this.logger.LogWarning("Address {AddressId} not found for user {UserId}", command.AddressId, command.UserId);
            throw new InvalidOperationException("Address not found.");
        }

        address.FullName = command.FullName;
        address.Phone = command.Phone;
        address.AddressLine = command.AddressLine;
        address.Ward = command.Ward;
        address.District = command.District;
        address.City = command.City;
        address.UpdatedAt = DateTime.UtcNow;

        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Updated address {AddressId} for user {UserId}", address.Id, command.UserId);

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
