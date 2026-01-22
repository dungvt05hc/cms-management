// <copyright file="GetShippingMethodsHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Shipping.GetShippingMethods;

/// <summary>
/// Handler for GetShippingMethodsQuery.
/// </summary>
public class GetShippingMethodsHandler
{
    private readonly IAppDbContext dbContext;
    private readonly ILogger<GetShippingMethodsHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetShippingMethodsHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger.</param>
    public GetShippingMethodsHandler(
        IAppDbContext dbContext,
        ILogger<GetShippingMethodsHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Handles getting all active shipping methods.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of shipping methods.</returns>
    public async Task<List<ShippingMethodDto>> Handle(
        GetShippingMethodsQuery query,
        CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Getting active shipping methods");

        var methods = await this.dbContext.ShippingMethods
            .Include(m => m.Carriers)
            .Where(m => m.IsActive)
            .OrderBy(m => m.Name)
            .ToListAsync(cancellationToken);

        var result = methods.Select(m => new ShippingMethodDto(
            m.Id,
            m.Code,
            m.Name,
            m.Description,
            m.Carriers
                .Where(c => c.IsActive)
                .Select(c => new ShippingCarrierDto(
                    c.Id,
                    c.Code,
                    c.Name,
                    c.Description,
                    c.SupportsCOD))
                .ToList()))
            .ToList();

        return result;
    }
}
