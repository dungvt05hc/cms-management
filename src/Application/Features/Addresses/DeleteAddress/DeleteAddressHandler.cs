// <copyright file="DeleteAddressHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Addresses.DeleteAddress;

/// <summary>
/// Handler for DeleteAddressCommand.
/// </summary>
public class DeleteAddressHandler
{
    private readonly IAppDbContext dbContext;
    private readonly ILogger<DeleteAddressHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteAddressHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger.</param>
    public DeleteAddressHandler(
        IAppDbContext dbContext,
        ILogger<DeleteAddressHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Handles deleting an address.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the operation.</returns>
    public async Task Handle(DeleteAddressCommand command, CancellationToken cancellationToken)
    {
        var address = await this.dbContext.Addresses
            .FirstOrDefaultAsync(a => a.Id == command.AddressId && a.UserId == command.UserId, cancellationToken);

        if (address == null)
        {
            this.logger.LogWarning("Address {AddressId} not found for user {UserId}", command.AddressId, command.UserId);
            throw new InvalidOperationException("Address not found.");
        }

        this.dbContext.Addresses.Remove(address);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Deleted address {AddressId} for user {UserId}", command.AddressId, command.UserId);
    }
}
