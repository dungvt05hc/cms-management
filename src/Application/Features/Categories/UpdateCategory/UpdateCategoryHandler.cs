// <copyright file="UpdateCategoryHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Categories.UpdateCategory;

/// <summary>
/// Handler for the UpdateCategoryCommand.
/// </summary>
public class UpdateCategoryHandler
{
    private readonly IAppDbContext dbContext;
    private readonly IDateTime dateTime;
    private readonly ILogger<UpdateCategoryHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCategoryHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="dateTime">The date time service.</param>
    /// <param name="logger">The logger.</param>
    public UpdateCategoryHandler(
        IAppDbContext dbContext,
        IDateTime dateTime,
        ILogger<UpdateCategoryHandler> logger)
    {
        this.dbContext = dbContext;
        this.dateTime = dateTime;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the update category command.
    /// </summary>
    /// <param name="command">The update category command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when category not found, parent not found, or cycle detected.</exception>
    public async Task Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await this.dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (category == null)
        {
            this.logger.LogWarning("Category not found: {CategoryId}", command.Id);
            throw new InvalidOperationException("Category not found.");
        }

        if (command.ParentId.HasValue)
        {
            // Prevent setting self as parent
            if (command.ParentId.Value == command.Id)
            {
                this.logger.LogWarning("Cycle detected: attempting to set self as parent. CategoryId: {CategoryId}", command.Id);
                throw new InvalidOperationException("A category cannot be its own parent.");
            }

            var parentExists = await this.dbContext.Categories
                .AnyAsync(c => c.Id == command.ParentId.Value, cancellationToken);

            if (!parentExists)
            {
                this.logger.LogWarning("Parent category not found: {ParentId}", command.ParentId.Value);
                throw new InvalidOperationException("Parent category not found.");
            }

            // Prevent cycle: check if new parent is a descendant
            if (await this.IsDescendant(command.Id, command.ParentId.Value, cancellationToken))
            {
                this.logger.LogWarning("Cycle detected: parent is a descendant. CategoryId: {CategoryId}, ParentId: {ParentId}", command.Id, command.ParentId.Value);
                throw new InvalidOperationException("Cannot set parent: cycle detected.");
            }
        }

        category.Name = command.Name;
        category.ParentId = command.ParentId;
        category.UpdatedAt = this.dateTime.UtcNow;

        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Category updated: {CategoryId}", category.Id);
    }

    private async Task<bool> IsDescendant(Guid ancestorId, Guid descendantId, CancellationToken cancellationToken)
    {
        var current = descendantId;

        while (true)
        {
            var category = await this.dbContext.Categories
                .FirstOrDefaultAsync(c => c.Id == current, cancellationToken);

            if (category == null || category.ParentId == null)
            {
                return false;
            }

            if (category.ParentId.Value == ancestorId)
            {
                return true;
            }

            current = category.ParentId.Value;
        }
    }
}
