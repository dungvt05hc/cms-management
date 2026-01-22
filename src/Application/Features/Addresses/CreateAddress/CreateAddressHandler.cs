// <copyright file="CreateAddressHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Addresses.CreateAddress;

/// <summary>
/// Handler for CreateAddressCommand.
/// </summary>
public class CreateAddressHandler
{
    private readonly IAppDbContext dbContext;
    private readonly ILogger<CreateAddressHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateAddressHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger.</param>
    public CreateAddressHandler(
        IAppDbContext dbContext,
        ILogger<CreateAddressHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Handles creating a new address.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created address.</returns>
    public async Task<AddressDto> Handle(CreateAddressCommand command, CancellationToken cancellationToken)
    {
        var existingCount = await this.dbContext.Addresses
            .CountAsync(a => a.UserId == command.UserId, cancellationToken);

        if (existingCount >= 5)
        {
            this.logger.LogWarning("User {UserId} already has 5 addresses", command.UserId);
            throw new InvalidOperationException("Maximum 5 addresses allowed per user.");
        }

        if (command.IsDefault)
        {
            var currentDefault = await this.dbContext.Addresses
                .Where(a => a.UserId == command.UserId && a.IsDefault)
                .ToListAsync(cancellationToken);

            foreach (var addr in currentDefault)
            {
                addr.IsDefault = false;
                addr.UpdatedAt = DateTime.UtcNow;
            }
        }

        var address = new Address
        {
            Id = Guid.NewGuid(),
            UserId = command.UserId,
            FullName = command.FullName,
            Phone = command.Phone,
            AddressLine = command.AddressLine,
            Ward = command.Ward,
            District = command.District,
            City = command.City,
            IsDefault = command.IsDefault,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        this.dbContext.Addresses.Add(address);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Created address {AddressId} for user {UserId}", address.Id, command.UserId);

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
