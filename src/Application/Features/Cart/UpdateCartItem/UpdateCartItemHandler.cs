// <copyright file="UpdateCartItemHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Cart.UpdateCartItem;

/// <summary>
/// Handler for UpdateCartItemCommand.
/// </summary>
public class UpdateCartItemHandler
{
    private readonly IAppDbContext dbContext;
    private readonly ILogger<UpdateCartItemHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCartItemHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger.</param>
    public UpdateCartItemHandler(
        IAppDbContext dbContext,
        ILogger<UpdateCartItemHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Handles updating a cart item.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated cart item.</returns>
    public async Task<CartItemDto> Handle(UpdateCartItemCommand command, CancellationToken cancellationToken)
    {
        var cartItem = await this.dbContext.CartItems
            .Include(i => i.Cart)
            .Include(i => i.Product)
                .ThenInclude(p => p!.Variants)
            .Include(i => i.Variant)
            .FirstOrDefaultAsync(i => i.Id == command.ItemId, cancellationToken);

        if (cartItem == null)
        {
            throw new InvalidOperationException("Cart item not found.");
        }

        if (cartItem.Cart!.UserId != command.UserId)
        {
            throw new UnauthorizedAccessException("You do not have permission to update this cart item.");
        }

        if (command.Quantity.HasValue)
        {
            cartItem.Quantity = command.Quantity.Value;
        }

        if (command.VariantId.HasValue)
        {
            var variant = cartItem.Product?.Variants.FirstOrDefault(v => v.Id == command.VariantId.Value);
            if (variant == null)
            {
                throw new InvalidOperationException("Variant not found.");
            }

            cartItem.VariantId = command.VariantId.Value;
            cartItem.Variant = variant;
        }

        if (command.Selected.HasValue)
        {
            cartItem.Selected = command.Selected.Value;
        }

        cartItem.UpdatedAt = DateTime.UtcNow;
        cartItem.Cart.UpdatedAt = DateTime.UtcNow;

        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Updated cart item {ItemId}", command.ItemId);

        var price = cartItem.Variant?.Price ?? cartItem.Product?.Variants.FirstOrDefault()?.Price ?? 0m;
        var lineTotal = price * cartItem.Quantity;

        return new CartItemDto(
            cartItem.Id,
            cartItem.ProductId,
            cartItem.Product?.Name ?? string.Empty,
            cartItem.Product?.Slug ?? string.Empty,
            cartItem.VariantId,
            cartItem.Variant?.VariantName,
            price,
            cartItem.Quantity,
            cartItem.Selected,
            lineTotal);
    }
}
