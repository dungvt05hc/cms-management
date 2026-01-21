// <copyright file="UpdateProductHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.UpdateProduct;

/// <summary>
/// Handler for the UpdateProductCommand.
/// </summary>
public class UpdateProductHandler
{
    private readonly IAppDbContext dbContext;
    private readonly IDateTime dateTime;
    private readonly ILogger<UpdateProductHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateProductHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="dateTime">The date time service.</param>
    /// <param name="logger">The logger.</param>
    public UpdateProductHandler(
        IAppDbContext dbContext,
        IDateTime dateTime,
        ILogger<UpdateProductHandler> logger)
    {
        this.dbContext = dbContext;
        this.dateTime = dateTime;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the update product command.
    /// </summary>
    /// <param name="command">The update product command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when product not found, slug or SKU is duplicate, or category not found.</exception>
    public async Task Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product = await this.dbContext.Products
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == command.ProductId, cancellationToken);

        if (product == null)
        {
            this.logger.LogWarning("Product not found: {ProductId}", command.ProductId);
            throw new InvalidOperationException("Product not found.");
        }

        if (product.Slug != command.Slug)
        {
            var slugExists = await this.dbContext.Products
                .AnyAsync(p => p.Slug == command.Slug && p.Id != command.ProductId, cancellationToken);

            if (slugExists)
            {
                this.logger.LogWarning("Product slug already exists: {Slug}", command.Slug);
                throw new InvalidOperationException($"Product slug '{command.Slug}' already exists.");
            }
        }

        var skus = command.Variants.Select(v => v.Sku).ToList();
        var existingSku = await this.dbContext.ProductVariants
            .Where(v => skus.Contains(v.Sku) && v.ProductId != command.ProductId)
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

        product.Name = command.Name;
        product.Slug = command.Slug;
        product.Description = command.Description;
        product.CategoryId = command.CategoryId;
        product.Images = command.Images;
        product.Videos = command.Videos;
        product.Specifications = command.Specifications;
        product.IsActive = command.IsActive;
        product.UpdatedAt = now;

        var existingVariantIds = command.Variants
            .Where(v => v.Id.HasValue)
            .Select(v => v.Id!.Value)
            .ToList();

        var variantsToRemove = product.Variants
            .Where(v => !existingVariantIds.Contains(v.Id))
            .ToList();

        foreach (var variant in variantsToRemove)
        {
            this.dbContext.ProductVariants.Remove(variant);
        }

        foreach (var variantDto in command.Variants)
        {
            if (variantDto.Id.HasValue)
            {
                var existingVariant = product.Variants.FirstOrDefault(v => v.Id == variantDto.Id.Value);
                if (existingVariant != null)
                {
                    existingVariant.Sku = variantDto.Sku;
                    existingVariant.VariantName = variantDto.VariantName;
                    existingVariant.Price = variantDto.Price;
                    existingVariant.StockQuantity = variantDto.StockQuantity;
                    existingVariant.UpdatedAt = now;
                }
            }
            else
            {
                var newVariant = new ProductVariant
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
                product.Variants.Add(newVariant);
            }
        }

        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Product updated: {ProductId}", product.Id);
    }
}
