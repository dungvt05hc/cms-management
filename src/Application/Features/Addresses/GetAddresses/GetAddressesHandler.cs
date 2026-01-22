// <copyright file="GetAddressesHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Addresses.GetAddresses;

/// <summary>
/// Handler for GetAddressesQuery.
/// </summary>
public class GetAddressesHandler
{
    private readonly IAppDbContext dbContext;
    private readonly ILogger<GetAddressesHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAddressesHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger.</param>
    public GetAddressesHandler(
        IAppDbContext dbContext,
        ILogger<GetAddressesHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the get addresses query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of addresses.</returns>
    public async Task<List<AddressDto>> Handle(GetAddressesQuery query, CancellationToken cancellationToken)
    {
        var addresses = await this.dbContext.Addresses
            .Where(a => a.UserId == query.UserId)
            .OrderByDescending(a => a.IsDefault)
            .ThenByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

        this.logger.LogInformation("Retrieved {Count} addresses for user {UserId}", addresses.Count, query.UserId);

        return addresses.Select(a => new AddressDto(
            a.Id,
            a.UserId,
            a.FullName,
            a.Phone,
            a.AddressLine,
            a.Ward,
            a.District,
            a.City,
            a.IsDefault,
            a.CreatedAt,
            a.UpdatedAt)).ToList();
    }
}
