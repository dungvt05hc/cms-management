// <copyright file="GetProductsHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.GetProducts;

/// <summary>
/// Handler for the GetProductsQuery.
/// </summary>
public class GetProductsHandler
{
    private readonly IAppDbContext dbContext;
    private readonly ILogger<GetProductsHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetProductsHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger.</param>
    public GetProductsHandler(
        IAppDbContext dbContext,
        ILogger<GetProductsHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the get products query.
    /// </summary>
    /// <param name="query">The get products query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of products.</returns>
    public async Task<PagedResult<ProductDto>> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        var productsQuery = this.dbContext.Products
            .Include(p => p.Variants)
            .AsQueryable();

        if (query.CategoryId.HasValue)
        {
            productsQuery = productsQuery.Where(p => p.CategoryId == query.CategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Q))
        {
            var searchTerm = query.Q.ToLower();
            productsQuery = productsQuery.Where(p =>
                p.Name.ToLower().Contains(searchTerm) ||
                p.Slug.ToLower().Contains(searchTerm));
        }

        var totalCount = await productsQuery.CountAsync(cancellationToken);

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 10 : (query.PageSize > 100 ? 100 : query.PageSize);
        var skip = (page - 1) * pageSize;

        var products = await productsQuery
            .OrderByDescending(p => p.CreatedAt)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var productDtos = products.Select(p => new ProductDto(
            p.Id,
            p.Name,
            p.Slug,
            p.Description,
            p.CategoryId,
            p.Images,
            p.Videos,
            p.Specifications,
            p.IsActive,
            p.Variants.Select(v => new ProductVariantDto(v.Id, v.Sku, v.VariantName, v.Price, v.StockQuantity)).ToList(),
            p.CreatedAt,
            p.UpdatedAt)).ToList();

        this.logger.LogInformation("Retrieved {Count} products (page {Page})", productDtos.Count, page);

        return new PagedResult<ProductDto>(productDtos, totalCount, page, pageSize);
    }
}
