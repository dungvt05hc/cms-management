// <copyright file="CreateCategoryHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Categories.CreateCategory;

/// <summary>
/// Handler for the CreateCategoryCommand.
/// </summary>
public class CreateCategoryHandler
{
    private readonly IAppDbContext dbContext;
    private readonly IDateTime dateTime;
    private readonly ILogger<CreateCategoryHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateCategoryHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="dateTime">The date time service.</param>
    /// <param name="logger">The logger.</param>
    public CreateCategoryHandler(
        IAppDbContext dbContext,
        IDateTime dateTime,
        ILogger<CreateCategoryHandler> logger)
    {
        this.dbContext = dbContext;
        this.dateTime = dateTime;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the create category command.
    /// </summary>
    /// <param name="command">The create category command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created category DTO.</returns>
    /// <exception cref="InvalidOperationException">Thrown when parent not found or cycle detected.</exception>
    public async Task<CategoryDto> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        if (command.ParentId.HasValue)
        {
            var parentExists = await this.dbContext.Categories
                .AnyAsync(c => c.Id == command.ParentId.Value, cancellationToken);

            if (!parentExists)
            {
                this.logger.LogWarning("Parent category not found: {ParentId}", command.ParentId.Value);
                throw new InvalidOperationException("Parent category not found.");
            }
        }

        var now = this.dateTime.UtcNow;
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            ParentId = command.ParentId,
            CreatedAt = now,
            UpdatedAt = now,
        };

        this.dbContext.Categories.Add(category);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Category created: {CategoryId}", category.Id);

        return new CategoryDto(category.Id, category.Name, category.ParentId, category.CreatedAt, category.UpdatedAt);
    }
}
