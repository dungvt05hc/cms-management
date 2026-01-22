// <copyright file="GetProductSuggestionsHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.GetProductSuggestions;

/// <summary>
/// Handler for the GetProductSuggestionsQuery.
/// </summary>
public class GetProductSuggestionsHandler
{
    private readonly IAppDbContext dbContext;
    private readonly ILogger<GetProductSuggestionsHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetProductSuggestionsHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger.</param>
    public GetProductSuggestionsHandler(
        IAppDbContext dbContext,
        ILogger<GetProductSuggestionsHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the get product suggestions query.
    /// Returns products from the same category, excluding the current product.
    /// </summary>
    /// <param name="query">The get product suggestions query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of product DTOs (suggestions).</returns>
    public async Task<List<ProductDto>> Handle(GetProductSuggestionsQuery query, CancellationToken cancellationToken)
    {
        // Sanitize slug before logging to prevent log forging
        var safeSlug = query.Slug?.Replace("\r", string.Empty).Replace("\n", string.Empty);

        // First, find the product to get its category
        var product = await this.dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Slug == query.Slug && p.IsActive, cancellationToken);

        if (product == null)
        {
            this.logger.LogInformation("Product not found for suggestions: {Slug}", safeSlug);
            return new List<ProductDto>();
        }

        // If product has no category, return empty list
        if (product.CategoryId == null)
        {
            this.logger.LogInformation("Product has no category for suggestions: {Slug}", safeSlug);
            return new List<ProductDto>();
        }

        // Get products from the same category, excluding the current product
        var suggestions = await this.dbContext.Products
            .Include(p => p.Variants)
            .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id && p.IsActive)
            .OrderBy(p => p.CreatedAt)
            .Take(query.Limit)
            .ToListAsync(cancellationToken);

        this.logger.LogInformation("Retrieved {Count} suggestions for product: {Slug}", suggestions.Count, safeSlug);

        return suggestions.Select(p => new ProductDto(
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
    }
}
