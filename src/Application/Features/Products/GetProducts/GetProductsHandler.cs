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
    /// Handles the get products query with cursor pagination.
    /// </summary>
    /// <param name="query">The get products query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A cursor-paginated list of products.</returns>
    public async Task<CursorPagedResult<ProductDto>> HandleCursor(GetProductsQuery query, CancellationToken cancellationToken)
    {
        var productsQuery = this.dbContext.Products
            .Include(p => p.Variants)
            .Include(p => p.Category)
            .AsQueryable();

        // Apply filters
        if (query.CategoryId.HasValue)
        {
            productsQuery = productsQuery.Where(p => p.CategoryId == query.CategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.CategorySlug))
        {
            productsQuery = productsQuery.Where(p => p.Category != null && p.Category.Name.ToLower() == query.CategorySlug.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(query.Q))
        {
            var searchTerm = query.Q.ToLower();
            productsQuery = productsQuery.Where(p =>
                p.Name.ToLower().Contains(searchTerm) ||
                p.Slug.ToLower().Contains(searchTerm));
        }

        if (query.Featured.HasValue)
        {
            productsQuery = productsQuery.Where(p => p.IsFeatured == query.Featured.Value);
        }

        // Parse cursor
        ProductCursor.CursorData? cursorData = null;
        if (!string.IsNullOrWhiteSpace(query.Cursor))
        {
            cursorData = ProductCursor.Decode(query.Cursor);
            if (cursorData == null)
            {
                this.logger.LogWarning("Invalid cursor provided");
            }
        }

        // Apply sorting and cursor filter
        var sortOrder = query.Sort?.ToLower();
        if (sortOrder == "priceasc")
        {
            if (cursorData != null)
            {
                // For price ascending: continue from last price/id
                productsQuery = productsQuery.Where(p =>
                    p.Variants.Any() &&
                    (p.Variants.Min(v => v.Price) > cursorData.Price ||
                     (p.Variants.Min(v => v.Price) == cursorData.Price && p.Id > cursorData.ProductId)));
            }

            productsQuery = productsQuery
                .Where(p => p.Variants.Any())
                .OrderBy(p => p.Variants.Min(v => v.Price))
                .ThenBy(p => p.Id);
        }
        else if (sortOrder == "pricedesc")
        {
            if (cursorData != null)
            {
                // For price descending: continue from last price/id
                productsQuery = productsQuery.Where(p =>
                    p.Variants.Any() &&
                    (p.Variants.Min(v => v.Price) < cursorData.Price ||
                     (p.Variants.Min(v => v.Price) == cursorData.Price && p.Id > cursorData.ProductId)));
            }

            productsQuery = productsQuery
                .Where(p => p.Variants.Any())
                .OrderByDescending(p => p.Variants.Min(v => v.Price))
                .ThenBy(p => p.Id);
        }
        else
        {
            // Default: sort by CreatedAt descending
            if (cursorData != null)
            {
                productsQuery = productsQuery.Where(p =>
                    p.CreatedAt < cursorData.CreatedAt ||
                    (p.CreatedAt == cursorData.CreatedAt && p.Id > cursorData.ProductId));
            }

            productsQuery = productsQuery
                .OrderByDescending(p => p.CreatedAt)
                .ThenBy(p => p.Id);
        }

        var limit = query.Limit ?? 20;
        if (limit < 1)
        {
            limit = 20;
        }

        if (limit > 100)
        {
            limit = 100;
        }

        // Fetch limit + 1 to check if there are more items
        var products = await productsQuery
            .Take(limit + 1)
            .ToListAsync(cancellationToken);

        var hasMore = products.Count > limit;
        if (hasMore)
        {
            products = products.Take(limit).ToList();
        }

        var productDtos = products.Select(p =>
        {
            var minPrice = p.Variants.Any() ? p.Variants.Min(v => v.Price) : 0;
            return new ProductDto(
                p.Id,
                p.Name,
                p.Slug,
                p.Description,
                p.CategoryId,
                p.Images,
                p.Videos,
                p.Specifications,
                p.IsActive,
                p.IsFeatured,
                p.Variants.Select(v => new ProductVariantDto(v.Id, v.Sku, v.VariantName, v.Price, v.StockQuantity)).ToList(),
                p.CreatedAt,
                p.UpdatedAt);
        }).ToList();

        // Generate next cursor
        string? nextCursor = null;
        if (hasMore && products.Any())
        {
            var lastProduct = products.Last();
            var lastPrice = lastProduct.Variants.Any() ? lastProduct.Variants.Min(v => v.Price) : 0;
            nextCursor = ProductCursor.Encode(lastProduct.Id, lastPrice, lastProduct.CreatedAt);
        }

        this.logger.LogInformation("Retrieved {Count} products with cursor pagination", productDtos.Count);

        return new CursorPagedResult<ProductDto>(productDtos, nextCursor, hasMore);
    }

    /// <summary>
    /// Handles the get products query (legacy offset pagination).
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

        if (query.Featured.HasValue)
        {
            productsQuery = productsQuery.Where(p => p.IsFeatured == query.Featured.Value);
        }

        var totalCount = await productsQuery.CountAsync(cancellationToken);

        var page = query.Page ?? 1;
        var pageSize = query.PageSize ?? 10;
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);
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
            p.IsFeatured,
            p.Variants.Select(v => new ProductVariantDto(v.Id, v.Sku, v.VariantName, v.Price, v.StockQuantity)).ToList(),
            p.CreatedAt,
            p.UpdatedAt)).ToList();

        this.logger.LogInformation("Retrieved {Count} products (page {Page})", productDtos.Count, page);

        return new PagedResult<ProductDto>(productDtos, totalCount, page, pageSize);
    }
}
