// <copyright file="CreateProductHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.CreateProduct;

/// <summary>
/// Handler for the CreateProductCommand.
/// </summary>
public class CreateProductHandler
{
    private readonly IAppDbContext dbContext;
    private readonly IDateTime dateTime;
    private readonly ILogger<CreateProductHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateProductHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="dateTime">The date time service.</param>
    /// <param name="logger">The logger.</param>
    public CreateProductHandler(
        IAppDbContext dbContext,
        IDateTime dateTime,
        ILogger<CreateProductHandler> logger)
    {
        this.dbContext = dbContext;
        this.dateTime = dateTime;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the create product command.
    /// </summary>
    /// <param name="command">The create product command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created product DTO.</returns>
    /// <exception cref="InvalidOperationException">Thrown when slug or SKU is duplicate or category not found.</exception>
    public async Task<ProductDto> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var slugExists = await this.dbContext.Products
            .AnyAsync(p => p.Slug == command.Slug, cancellationToken);

        if (slugExists)
        {
            this.logger.LogWarning("Product slug already exists: {Slug}", command.Slug);
            throw new InvalidOperationException($"Product slug '{command.Slug}' already exists.");
        }

        var skus = command.Variants.Select(v => v.Sku).ToList();
        var existingSku = await this.dbContext.ProductVariants
            .Where(v => skus.Contains(v.Sku))
            .Select(v => v.Sku)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingSku != null)
        {
            this.logger.LogWarning("Product variant SKU already exists: {Sku}", existingSku);
            throw new InvalidOperationException($"Product variant SKU '{existingSku}' already exists.");
        }

        if (command.CategoryId.HasValue)
        {
            var categoryExists = await this.dbContext.Categories
                .AnyAsync(c => c.Id == command.CategoryId.Value, cancellationToken);

            if (!categoryExists)
            {
                this.logger.LogWarning("Category not found: {CategoryId}", command.CategoryId.Value);
                throw new InvalidOperationException("Category not found.");
            }
        }

        var now = this.dateTime.UtcNow;
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Slug = command.Slug,
            Description = command.Description,
            CategoryId = command.CategoryId,
            Images = command.Images,
            Videos = command.Videos,
            Specifications = command.Specifications,
            IsActive = command.IsActive,
            IsFeatured = command.IsFeatured,
            CreatedAt = now,
            UpdatedAt = now,
        };

        foreach (var variantDto in command.Variants)
        {
            var variant = new ProductVariant
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Sku = variantDto.Sku,
                VariantName = variantDto.VariantName,
                Price = variantDto.Price,
                StockQuantity = variantDto.StockQuantity,
                CreatedAt = now,
                UpdatedAt = now,
            };
            product.Variants.Add(variant);
        }

        this.dbContext.Products.Add(product);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Product created: {ProductId}", product.Id);

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
