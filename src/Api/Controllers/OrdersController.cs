// <copyright file="OrdersController.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Security.Claims;

using Application.Features.Orders;
using Application.Features.Orders.CancelOrder;
using Application.Features.Orders.ConfirmOrder;
using Application.Features.Orders.GetOrderById;
using Application.Features.Orders.GetOrders;
using Application.Features.Orders.Reorder;
using Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Orders controller for authenticated buyers.
/// </summary>
[ApiController]
[Route("me/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly GetOrdersHandler getOrdersHandler;
    private readonly GetOrderByIdHandler getOrderByIdHandler;
    private readonly CancelOrderHandler cancelOrderHandler;
    private readonly IValidator<CancelOrderCommand> cancelOrderValidator;
    private readonly ConfirmOrderHandler confirmOrderHandler;
    private readonly ReorderHandler reorderHandler;
    private readonly IValidator<ReorderCommand> reorderValidator;
    private readonly ILogger<OrdersController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrdersController"/> class.
    /// </summary>
    /// <param name="getOrdersHandler">The get orders handler.</param>
    /// <param name="getOrderByIdHandler">The get order by ID handler.</param>
    /// <param name="cancelOrderHandler">The cancel order handler.</param>
    /// <param name="cancelOrderValidator">The cancel order validator.</param>
    /// <param name="confirmOrderHandler">The confirm order handler.</param>
    /// <param name="reorderHandler">The reorder handler.</param>
    /// <param name="reorderValidator">The reorder validator.</param>
    /// <param name="logger">The logger.</param>
    public OrdersController(
        GetOrdersHandler getOrdersHandler,
        GetOrderByIdHandler getOrderByIdHandler,
        CancelOrderHandler cancelOrderHandler,
        IValidator<CancelOrderCommand> cancelOrderValidator,
        ConfirmOrderHandler confirmOrderHandler,
        ReorderHandler reorderHandler,
        IValidator<ReorderCommand> reorderValidator,
        ILogger<OrdersController> logger)
    {
        this.getOrdersHandler = getOrdersHandler;
        this.getOrderByIdHandler = getOrderByIdHandler;
        this.cancelOrderHandler = cancelOrderHandler;
        this.cancelOrderValidator = cancelOrderValidator;
        this.confirmOrderHandler = confirmOrderHandler;
        this.reorderHandler = reorderHandler;
        this.reorderValidator = reorderValidator;
        this.logger = logger;
    }

    /// <summary>
    /// Get user's orders with optional status filter.
    /// </summary>
    /// <param name="status">Optional status filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of orders.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetOrders(
        [FromQuery] OrderStatus? status,
        CancellationToken cancellationToken)
    {
        var userId = this.GetUserId();
        if (!userId.HasValue)
        {
            return this.Unauthorized(new { Message = "Invalid token." });
        }

        var query = new GetOrdersQuery(userId.Value, status);
        var orders = await this.getOrdersHandler.Handle(query, cancellationToken);

        return this.Ok(orders);
    }

    /// <summary>
    /// Get order details by ID.
    /// </summary>
    /// <param name="id">The order ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The order details.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetOrderById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var userId = this.GetUserId();
        if (!userId.HasValue)
        {
            return this.Unauthorized(new { Message = "Invalid token." });
        }

        var query = new GetOrderByIdQuery(userId.Value, id);
        var order = await this.getOrderByIdHandler.Handle(query, cancellationToken);

        if (order == null)
        {
            return this.NotFound(new { Message = "Order not found." });
        }

        return this.Ok(order);
    }

    /// <summary>
    /// Cancel an order (only if status is Processing).
    /// </summary>
    /// <param name="id">The order ID.</param>
    /// <param name="request">The cancel request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CancelOrder(
        [FromRoute] Guid id,
        [FromBody] CancelOrderRequest request,
        CancellationToken cancellationToken)
    {
        var userId = this.GetUserId();
        if (!userId.HasValue)
        {
            return this.Unauthorized(new { Message = "Invalid token." });
        }

        var command = new CancelOrderCommand(userId.Value, id, request.ReasonCode, request.Note);

        var validationResult = await this.cancelOrderValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return this.BadRequest(new
            {
                Message = "Validation failed.",
                Errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }),
            });
        }

        try
        {
            await this.cancelOrderHandler.Handle(command, cancellationToken);
            return this.NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return this.BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Confirm order received (only if status is Delivered).
    /// </summary>
    /// <param name="id">The order ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpPost("{id}/confirm-received")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ConfirmOrderReceived(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var userId = this.GetUserId();
        if (!userId.HasValue)
        {
            return this.Unauthorized(new { Message = "Invalid token." });
        }

        var command = new ConfirmOrderCommand(userId.Value, id);

        try
        {
            await this.confirmOrderHandler.Handle(command, cancellationToken);
            return this.NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return this.BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Reorder items from an existing order (Delivered or Cancelled) to cart.
    /// </summary>
    /// <param name="id">The order ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Cart ID where items were added.</returns>
    [HttpPost("{id}/reorder")]
    [ProducesResponseType(typeof(ReorderResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Reorder(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var userId = this.GetUserId();
        if (!userId.HasValue)
        {
            return this.Unauthorized(new { Message = "Invalid token." });
        }

        var command = new ReorderCommand(userId.Value, id);

        var validationResult = await this.reorderValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return this.BadRequest(new
            {
                Message = "Validation failed.",
                Errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }),
            });
        }

        try
        {
            var result = await this.reorderHandler.Handle(command, cancellationToken);
            return this.Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return this.BadRequest(new { Message = ex.Message });
        }
    }

    private Guid? GetUserId()
    {
        var userIdClaim = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return null;
        }

        return userId;
    }
}
