// <copyright file="CreateProductGroupHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.ProductGroups.CreateProductGroup;

/// <summary>
/// Handler for the CreateProductGroupCommand.
/// </summary>
public class CreateProductGroupHandler
{
    private readonly IAppDbContext dbContext;
    private readonly IDateTime dateTime;
    private readonly ILogger<CreateProductGroupHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateProductGroupHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="dateTime">The date time service.</param>
    /// <param name="logger">The logger.</param>
    public CreateProductGroupHandler(
        IAppDbContext dbContext,
        IDateTime dateTime,
        ILogger<CreateProductGroupHandler> logger)
    {
        this.dbContext = dbContext;
        this.dateTime = dateTime;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the create product group command.
    /// </summary>
    /// <param name="command">The create product group command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created product group DTO.</returns>
    /// <exception cref="InvalidOperationException">Thrown when category not found.</exception>
    public async Task<ProductGroupDto> Handle(CreateProductGroupCommand command, CancellationToken cancellationToken)
    {
        var categoryExists = await this.dbContext.Categories
            .AnyAsync(c => c.Id == command.CategoryId, cancellationToken);

        if (!categoryExists)
        {
            this.logger.LogWarning("Category not found: {CategoryId}", command.CategoryId);
            throw new InvalidOperationException("Category not found.");
        }

        var now = this.dateTime.UtcNow;
        var productGroupId = Guid.NewGuid();

        var productGroup = new ProductGroup
        {
            Id = productGroupId,
            CategoryId = command.CategoryId,
            Name = command.Name,
            CreatedAt = now,
            UpdatedAt = now,
        };

        foreach (var attrDto in command.Attributes)
        {
            productGroup.Attributes.Add(new ProductGroupAttribute
            {
                Id = Guid.NewGuid(),
                ProductGroupId = productGroupId,
                Name = attrDto.Name,
                Key = attrDto.Key,
                Type = attrDto.Type,
                CreatedAt = now,
                UpdatedAt = now,
            });
        }

        this.dbContext.ProductGroups.Add(productGroup);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation(
            "Product group created: {ProductGroupId}, CategoryId: {CategoryId}",
            productGroup.Id,
            productGroup.CategoryId);

        return new ProductGroupDto(
            productGroup.Id,
            productGroup.CategoryId,
            productGroup.Name,
            productGroup.Attributes.Select(a => new ProductGroupAttributeDto(a.Name, a.Key, a.Type)).ToList(),
            productGroup.CreatedAt,
            productGroup.UpdatedAt);
    }
}
