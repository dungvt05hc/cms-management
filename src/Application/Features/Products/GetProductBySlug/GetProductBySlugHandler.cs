// <copyright file="GetProductBySlugHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.GetProductBySlug;

/// <summary>
/// Handler for the GetProductBySlugQuery.
/// </summary>
public class GetProductBySlugHandler
{
    private readonly IAppDbContext dbContext;
    private readonly ILogger<GetProductBySlugHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetProductBySlugHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger.</param>
    public GetProductBySlugHandler(
        IAppDbContext dbContext,
        ILogger<GetProductBySlugHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the get product by slug query.
    /// </summary>
    /// <param name="query">The get product by slug query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product DTO, or null if not found.</returns>
    public async Task<ProductDto?> Handle(GetProductBySlugQuery query, CancellationToken cancellationToken)
    {
        var product = await this.dbContext.Products
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Slug == query.Slug && p.IsActive, cancellationToken);

        if (product == null)
        {
            this.logger.LogInformation("Product not found for slug: {Slug}", query.Slug);
            return null;
        }

        this.logger.LogInformation("Retrieved product by slug: {Slug}", query.Slug);

        return new ProductDto(
            product.Id,
            product.Name,
            product.Slug,
            product.Description,
            product.CategoryId,
            product.Images,
            product.Videos,
            product.Specifications,
            product.IsActive,
            product.IsFeatured,
            product.Variants.Select(v => new ProductVariantDto(v.Id, v.Sku, v.VariantName, v.Price, v.StockQuantity)).ToList(),
            product.CreatedAt,
            product.UpdatedAt);
    }
}
