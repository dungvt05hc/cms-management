// <copyright file="UpdateProductGroupHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.ProductGroups.UpdateProductGroup;

/// <summary>
/// Handler for the UpdateProductGroupCommand.
/// </summary>
public class UpdateProductGroupHandler
{
    private readonly IAppDbContext dbContext;
    private readonly IDateTime dateTime;
    private readonly ILogger<UpdateProductGroupHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateProductGroupHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="dateTime">The date time service.</param>
    /// <param name="logger">The logger.</param>
    public UpdateProductGroupHandler(
        IAppDbContext dbContext,
        IDateTime dateTime,
        ILogger<UpdateProductGroupHandler> logger)
    {
        this.dbContext = dbContext;
        this.dateTime = dateTime;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the update product group command.
    /// </summary>
    /// <param name="command">The update product group command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when product group or category not found.</exception>
    public async Task Handle(UpdateProductGroupCommand command, CancellationToken cancellationToken)
    {
        var productGroup = await this.dbContext.ProductGroups
            .Include(pg => pg.Attributes)
            .FirstOrDefaultAsync(pg => pg.Id == command.Id, cancellationToken);

        if (productGroup == null)
        {
            this.logger.LogWarning("Product group not found: {ProductGroupId}", command.Id);
            throw new InvalidOperationException("Product group not found.");
        }

        var categoryExists = await this.dbContext.Categories
            .AnyAsync(c => c.Id == command.CategoryId, cancellationToken);

        if (!categoryExists)
        {
            this.logger.LogWarning("Category not found: {CategoryId}", command.CategoryId);
            throw new InvalidOperationException("Category not found.");
        }

        productGroup.CategoryId = command.CategoryId;
        productGroup.Name = command.Name;
        productGroup.UpdatedAt = this.dateTime.UtcNow;

        // Remove old attributes manually for InMemory database compatibility
        var existingAttributes = await this.dbContext.ProductGroupAttributes
            .Where(a => a.ProductGroupId == productGroup.Id)
            .ToListAsync(cancellationToken);

        foreach (var attr in existingAttributes)
        {
            this.dbContext.ProductGroupAttributes.Remove(attr);
        }

        // Add new attributes
        var now = this.dateTime.UtcNow;
        foreach (var attrDto in command.Attributes)
        {
            var newAttr = new ProductGroupAttribute
            {
                Id = Guid.NewGuid(),
                ProductGroupId = productGroup.Id,
                Name = attrDto.Name,
                Key = attrDto.Key,
                Type = attrDto.Type,
                CreatedAt = now,
                UpdatedAt = now,
            };
            this.dbContext.ProductGroupAttributes.Add(newAttr);
        }

        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation(
            "Product group updated: {ProductGroupId}, CategoryId: {CategoryId}",
            productGroup.Id,
            productGroup.CategoryId);
    }
}
