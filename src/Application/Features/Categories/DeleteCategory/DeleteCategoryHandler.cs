// <copyright file="DeleteCategoryHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Categories.DeleteCategory;

/// <summary>
/// Handler for the DeleteCategoryCommand.
/// </summary>
public class DeleteCategoryHandler
{
    private readonly IAppDbContext dbContext;
    private readonly ILogger<DeleteCategoryHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteCategoryHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger.</param>
    public DeleteCategoryHandler(
        IAppDbContext dbContext,
        ILogger<DeleteCategoryHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the delete category command.
    /// </summary>
    /// <param name="command">The delete category command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when category not found or has children.</exception>
    public async Task Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await this.dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (category == null)
        {
            this.logger.LogWarning("Category not found: {CategoryId}", command.Id);
            throw new InvalidOperationException("Category not found.");
        }

        var hasChildren = await this.dbContext.Categories
            .AnyAsync(c => c.ParentId == command.Id, cancellationToken);

        if (hasChildren)
        {
            this.logger.LogWarning("Cannot delete category with children: {CategoryId}", command.Id);
            throw new InvalidOperationException("Cannot delete a category that has child categories.");
        }

        this.dbContext.Categories.Remove(category);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Category deleted: {CategoryId}", command.Id);
    }
}
