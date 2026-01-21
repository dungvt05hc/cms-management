// <copyright file="DeleteProductHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Products.DeleteProduct;

/// <summary>
/// Handler for the DeleteProductCommand.
/// </summary>
public class DeleteProductHandler
{
    private readonly IAppDbContext dbContext;
    private readonly ILogger<DeleteProductHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteProductHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger.</param>
    public DeleteProductHandler(
        IAppDbContext dbContext,
        ILogger<DeleteProductHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the delete product command.
    /// </summary>
    /// <param name="command">The delete product command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when product not found.</exception>
    public async Task Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var product = await this.dbContext.Products
            .FirstOrDefaultAsync(p => p.Id == command.ProductId, cancellationToken);

        if (product == null)
        {
            this.logger.LogWarning("Product not found: {ProductId}", command.ProductId);
            throw new InvalidOperationException("Product not found.");
        }

        this.dbContext.Products.Remove(product);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Product deleted: {ProductId}", command.ProductId);
    }
}
