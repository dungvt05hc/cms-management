// <copyright file="GetInvoiceProfilesHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.InvoiceProfiles.GetInvoiceProfiles;

/// <summary>
/// Handler for GetInvoiceProfilesQuery.
/// </summary>
public class GetInvoiceProfilesHandler
{
    private readonly IAppDbContext dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetInvoiceProfilesHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public GetInvoiceProfilesHandler(IAppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    /// <summary>
    /// Handles getting invoice profiles for a user.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of invoice profiles.</returns>
    public async Task<List<InvoiceProfileDto>> Handle(
        GetInvoiceProfilesQuery query,
        CancellationToken cancellationToken)
    {
        var profiles = await this.dbContext.InvoiceProfiles
            .Where(p => p.UserId == query.UserId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new InvoiceProfileDto(
                p.Id,
                p.UserId,
                p.TaxCode,
                p.CompanyName,
                p.CompanyAddress,
                p.Email,
                p.CreatedAt,
                p.UpdatedAt))
            .ToListAsync(cancellationToken);

        return profiles;
    }
}
