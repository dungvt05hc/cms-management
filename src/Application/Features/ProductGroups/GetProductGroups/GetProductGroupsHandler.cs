// <copyright file="GetProductGroupsHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Application.Features.ProductGroups.CreateProductGroup;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProductGroups.GetProductGroups;

/// <summary>
/// Handler for the GetProductGroupsQuery.
/// </summary>
public class GetProductGroupsHandler
{
    private readonly IAppDbContext dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetProductGroupsHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public GetProductGroupsHandler(IAppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    /// <summary>
    /// Handles the get product groups query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of product groups.</returns>
    public async Task<List<ProductGroupDto>> Handle(GetProductGroupsQuery query, CancellationToken cancellationToken)
    {
        var productGroupsQuery = this.dbContext.ProductGroups
            .Include(pg => pg.Attributes)
            .AsQueryable();

        if (query.CategoryId.HasValue)
        {
            productGroupsQuery = productGroupsQuery.Where(pg => pg.CategoryId == query.CategoryId.Value);
        }

        var productGroups = await productGroupsQuery
            .OrderBy(pg => pg.Name)
            .ToListAsync(cancellationToken);

        return productGroups.Select(pg => new ProductGroupDto(
            pg.Id,
            pg.CategoryId,
            pg.Name,
            pg.Attributes.Select(a => new ProductGroupAttributeDto(a.Name, a.Key, a.Type)).ToList(),
            pg.CreatedAt,
            pg.UpdatedAt)).ToList();
    }
}
