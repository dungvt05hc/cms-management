// <copyright file="ConfirmOrderHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.ConfirmOrder;

/// <summary>
/// Handler for confirming order received.
/// </summary>
public class ConfirmOrderHandler
{
    private readonly IAppDbContext dbContext;
    private readonly IDateTime dateTime;
    private readonly ILogger<ConfirmOrderHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfirmOrderHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="dateTime">The date/time service.</param>
    /// <param name="logger">The logger.</param>
    public ConfirmOrderHandler(IAppDbContext dbContext, IDateTime dateTime, ILogger<ConfirmOrderHandler> logger)
    {
        this.dbContext = dbContext;
        this.dateTime = dateTime;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the confirm order command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when order not found or cannot be confirmed.</exception>
    public async Task Handle(ConfirmOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await this.dbContext.Orders
            .FirstOrDefaultAsync(o => o.Id == command.OrderId && o.UserId == command.UserId, cancellationToken);

        if (order == null)
        {
            throw new InvalidOperationException("Order not found.");
        }

        if (order.Status != OrderStatus.Delivered)
        {
            throw new InvalidOperationException("Order can only be confirmed when status is Delivered.");
        }

        order.UpdatedAt = this.dateTime.UtcNow;

        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Order {OrderId} confirmed by user {UserId}", command.OrderId, command.UserId);
    }
}
