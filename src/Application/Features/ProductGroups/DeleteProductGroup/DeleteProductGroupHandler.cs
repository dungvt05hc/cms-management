// <copyright file="DeleteProductGroupHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.ProductGroups.DeleteProductGroup;

/// <summary>
/// Handler for the DeleteProductGroupCommand.
/// </summary>
public class DeleteProductGroupHandler
{
    private readonly IAppDbContext dbContext;
    private readonly ILogger<DeleteProductGroupHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteProductGroupHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger.</param>
    public DeleteProductGroupHandler(
        IAppDbContext dbContext,
        ILogger<DeleteProductGroupHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the delete product group command.
    /// </summary>
    /// <param name="command">The delete product group command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when product group not found.</exception>
    public async Task Handle(DeleteProductGroupCommand command, CancellationToken cancellationToken)
    {
        var productGroup = await this.dbContext.ProductGroups
            .FirstOrDefaultAsync(pg => pg.Id == command.Id, cancellationToken);

        if (productGroup == null)
        {
            this.logger.LogWarning("Product group not found: {ProductGroupId}", command.Id);
            throw new InvalidOperationException("Product group not found.");
        }

        this.dbContext.ProductGroups.Remove(productGroup);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Product group deleted: {ProductGroupId}", command.Id);
    }
}
