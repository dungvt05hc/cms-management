// <copyright file="CancelOrderHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.CancelOrder;

/// <summary>
/// Handler for canceling an order.
/// </summary>
public class CancelOrderHandler
{
    private readonly IAppDbContext dbContext;
    private readonly IDateTime dateTime;
    private readonly ILogger<CancelOrderHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CancelOrderHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="dateTime">The date/time service.</param>
    /// <param name="logger">The logger.</param>
    public CancelOrderHandler(IAppDbContext dbContext, IDateTime dateTime, ILogger<CancelOrderHandler> logger)
    {
        this.dbContext = dbContext;
        this.dateTime = dateTime;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the cancel order command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when order not found or cannot be cancelled.</exception>
    public async Task Handle(CancelOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await this.dbContext.Orders
            .FirstOrDefaultAsync(o => o.Id == command.OrderId && o.UserId == command.UserId, cancellationToken);

        if (order == null)
        {
            throw new InvalidOperationException("Order not found.");
        }

        if (order.Status != OrderStatus.Processing)
        {
            throw new InvalidOperationException("Order can only be cancelled when status is Processing.");
        }

        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = this.dateTime.UtcNow;

        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Order {OrderId} cancelled by user {UserId}", command.OrderId, command.UserId);
    }
}
