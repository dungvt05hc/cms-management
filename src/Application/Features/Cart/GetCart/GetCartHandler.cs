// <copyright file="GetCartHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Cart.GetCart;

/// <summary>
/// Handler for GetCartQuery.
/// </summary>
public class GetCartHandler
{
    private readonly IAppDbContext dbContext;
    private readonly ILogger<GetCartHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCartHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger.</param>
    public GetCartHandler(
        IAppDbContext dbContext,
        ILogger<GetCartHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the get cart query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cart with items and subtotal.</returns>
    public async Task<CartDto> Handle(GetCartQuery query, CancellationToken cancellationToken)
    {
        var cart = await this.dbContext.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p!.Variants)
            .Include(c => c.Items)
                .ThenInclude(i => i.Variant)
            .FirstOrDefaultAsync(c => c.UserId == query.UserId, cancellationToken);

        if (cart == null)
        {
            cart = new Domain.Entities.Cart
            {
                Id = Guid.NewGuid(),
                UserId = query.UserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            this.dbContext.Carts.Add(cart);
            await this.dbContext.SaveChangesAsync(cancellationToken);
            this.logger.LogInformation("Created new cart for user {UserId}", query.UserId);
        }

        var items = cart.Items.Select(item =>
        {
            var price = item.Variant?.Price ?? item.Product?.Variants.FirstOrDefault()?.Price ?? 0m;
            var lineTotal = price * item.Quantity;
            return new CartItemDto(
                item.Id,
                item.ProductId,
                item.Product?.Name ?? string.Empty,
                item.Product?.Slug ?? string.Empty,
                item.VariantId,
                item.Variant?.VariantName,
                price,
                item.Quantity,
                item.Selected,
                lineTotal);
        }).ToList();

        var subtotal = items.Where(i => i.Selected).Sum(i => i.LineTotal);

        this.logger.LogInformation("Retrieved cart for user {UserId} with {ItemCount} items", query.UserId, items.Count);

        return new CartDto(cart.Id, cart.UserId, items, subtotal);
    }
}
