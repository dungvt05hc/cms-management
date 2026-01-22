// <copyright file="AddCartItemHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Cart.AddCartItem;

/// <summary>
/// Handler for AddCartItemCommand.
/// </summary>
public class AddCartItemHandler
{
    private readonly IAppDbContext dbContext;
    private readonly ILogger<AddCartItemHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddCartItemHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger.</param>
    public AddCartItemHandler(
        IAppDbContext dbContext,
        ILogger<AddCartItemHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Handles adding an item to cart.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created cart item.</returns>
    public async Task<CartItemDto> Handle(AddCartItemCommand command, CancellationToken cancellationToken)
    {
        var product = await this.dbContext.Products
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == command.ProductId, cancellationToken);

        if (product == null)
        {
            throw new InvalidOperationException("Product not found.");
        }

        if (command.VariantId.HasValue)
        {
            var variantToCheck = product.Variants.FirstOrDefault(v => v.Id == command.VariantId.Value);
            if (variantToCheck == null)
            {
                throw new InvalidOperationException("Variant not found.");
            }
        }

        var cart = await this.dbContext.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == command.UserId, cancellationToken);

        if (cart == null)
        {
            cart = new Domain.Entities.Cart
            {
                Id = Guid.NewGuid(),
                UserId = command.UserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            this.dbContext.Carts.Add(cart);
        }

        var existingItem = cart.Items.FirstOrDefault(i =>
            i.ProductId == command.ProductId &&
            i.VariantId == command.VariantId);

        if (existingItem != null)
        {
            existingItem.Quantity += command.Quantity;
            existingItem.UpdatedAt = DateTime.UtcNow;
            this.logger.LogInformation("Updated existing cart item {ItemId} quantity to {Quantity}", existingItem.Id, existingItem.Quantity);
        }
        else
        {
            var newItem = new Domain.Entities.CartItem
            {
                Id = Guid.NewGuid(),
                CartId = cart.Id,
                ProductId = command.ProductId,
                VariantId = command.VariantId,
                Quantity = command.Quantity,
                Selected = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            this.dbContext.CartItems.Add(newItem);
            existingItem = newItem;
            this.logger.LogInformation("Added new cart item {ItemId}", newItem.Id);
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await this.dbContext.SaveChangesAsync(cancellationToken);

        var variant = command.VariantId.HasValue
            ? product.Variants.FirstOrDefault(v => v.Id == command.VariantId.Value)
            : null;

        var price = variant?.Price ?? product.Variants.FirstOrDefault()?.Price ?? 0m;
        var lineTotal = price * existingItem.Quantity;

        return new CartItemDto(
            existingItem.Id,
            product.Id,
            product.Name,
            product.Slug,
            variant?.Id,
            variant?.VariantName,
            price,
            existingItem.Quantity,
            existingItem.Selected,
            lineTotal);
    }
}
