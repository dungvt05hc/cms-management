// <copyright file="DeleteCartItemHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Cart.DeleteCartItem;

/// <summary>
/// Handler for DeleteCartItemCommand.
/// </summary>
public class DeleteCartItemHandler
{
    private readonly IAppDbContext dbContext;
    private readonly ILogger<DeleteCartItemHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteCartItemHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger.</param>
    public DeleteCartItemHandler(
        IAppDbContext dbContext,
        ILogger<DeleteCartItemHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Handles deleting a cart item.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the async operation.</returns>
    public async Task Handle(DeleteCartItemCommand command, CancellationToken cancellationToken)
    {
        var cartItem = await this.dbContext.CartItems
            .Include(i => i.Cart)
            .FirstOrDefaultAsync(i => i.Id == command.ItemId, cancellationToken);

        if (cartItem == null)
        {
            throw new InvalidOperationException("Cart item not found.");
        }

        if (cartItem.Cart!.UserId != command.UserId)
        {
            throw new UnauthorizedAccessException("You do not have permission to delete this cart item.");
        }

        this.dbContext.CartItems.Remove(cartItem);
        cartItem.Cart.UpdatedAt = DateTime.UtcNow;

        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Deleted cart item {ItemId}", command.ItemId);
    }
}
