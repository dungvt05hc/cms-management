// <copyright file="ReorderHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Reorder;

/// <summary>
/// Handler for ReorderCommand.
/// </summary>
public class ReorderHandler
{
    private readonly IAppDbContext dbContext;
    private readonly ILogger<ReorderHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReorderHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger.</param>
    public ReorderHandler(
        IAppDbContext dbContext,
        ILogger<ReorderHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Handles reordering items from an existing order to cart.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cart ID where items were added.</returns>
    public async Task<ReorderResult> Handle(ReorderCommand command, CancellationToken cancellationToken)
    {
        // Find the order and verify ownership
        var order = await this.dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == command.OrderId && o.UserId == command.UserId, cancellationToken);

        if (order == null)
        {
            throw new InvalidOperationException("Order not found.");
        }

        // Only allow reorder for Delivered or Cancelled orders
        if (order.Status != OrderStatus.Delivered && order.Status != OrderStatus.Cancelled)
        {
            throw new InvalidOperationException("Only delivered or cancelled orders can be reordered.");
        }

        // Get or create the user's cart
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

        // Add each order item to the cart
        foreach (var orderItem in order.Items)
        {
            // Check if product still exists
            var productExists = await this.dbContext.Products
                .AnyAsync(p => p.Id == orderItem.ProductId, cancellationToken);

            if (!productExists)
            {
                this.logger.LogWarning(
                    "Product {ProductId} from order {OrderId} no longer exists, skipping",
                    orderItem.ProductId,
                    order.Id);
                continue;
            }

            // If variant was specified, check it still exists
            if (orderItem.VariantId.HasValue)
            {
                var variantExists = await this.dbContext.ProductVariants
                    .AnyAsync(v => v.Id == orderItem.VariantId.Value && v.ProductId == orderItem.ProductId, cancellationToken);

                if (!variantExists)
                {
                    this.logger.LogWarning(
                        "Variant {VariantId} for product {ProductId} from order {OrderId} no longer exists, skipping",
                        orderItem.VariantId,
                        orderItem.ProductId,
                        order.Id);
                    continue;
                }
            }

            // Check if the same product+variant already exists in cart
            var existingCartItem = cart.Items.FirstOrDefault(i =>
                i.ProductId == orderItem.ProductId &&
                i.VariantId == orderItem.VariantId);

            if (existingCartItem != null)
            {
                // Merge: add quantity
                existingCartItem.Quantity += orderItem.Quantity;
                existingCartItem.UpdatedAt = DateTime.UtcNow;
                this.logger.LogInformation(
                    "Merged order item into existing cart item {CartItemId}, new quantity: {Quantity}",
                    existingCartItem.Id,
                    existingCartItem.Quantity);
            }
            else
            {
                // Add new cart item
                var newCartItem = new Domain.Entities.CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    ProductId = orderItem.ProductId,
                    VariantId = orderItem.VariantId,
                    Quantity = orderItem.Quantity,
                    Selected = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };
                this.dbContext.CartItems.Add(newCartItem);
                this.logger.LogInformation(
                    "Added new cart item {CartItemId} from order {OrderId}",
                    newCartItem.Id,
                    order.Id);
            }
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation(
            "Reordered items from order {OrderId} to cart {CartId}",
            order.Id,
            cart.Id);

        return new ReorderResult(cart.Id);
    }
}
