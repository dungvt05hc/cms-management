// <copyright file="GetProductByIdHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.GetProductById;

/// <summary>
/// Handler for the GetProductByIdQuery.
/// </summary>
public class GetProductByIdHandler
{
    private readonly IAppDbContext dbContext;
    private readonly ILogger<GetProductByIdHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetProductByIdHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger.</param>
    public GetProductByIdHandler(
        IAppDbContext dbContext,
        ILogger<GetProductByIdHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the get product by ID query.
    /// </summary>
    /// <param name="query">The get product by ID query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product DTO.</returns>
    /// <exception cref="InvalidOperationException">Thrown when product not found.</exception>
    public async Task<ProductDto> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var product = await this.dbContext.Products
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == query.ProductId, cancellationToken);

        if (product == null)
        {
            this.logger.LogWarning("Product not found: {ProductId}", query.ProductId);
            throw new InvalidOperationException("Product not found.");
        }

        this.logger.LogInformation("Retrieved product: {ProductId}", product.Id);

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
            product.Variants.Select(v => new ProductVariantDto(v.Id, v.Sku, v.VariantName, v.Price, v.StockQuantity)).ToList(),
            product.CreatedAt,
            product.UpdatedAt);
    }
}
