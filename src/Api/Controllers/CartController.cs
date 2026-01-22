// <copyright file="CartController.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Security.Claims;

using Application.Features.Cart;
using Application.Features.Cart.AddCartItem;
using Application.Features.Cart.DeleteCartItem;
using Application.Features.Cart.GetCart;
using Application.Features.Cart.UpdateCartItem;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Cart controller for authenticated buyers.
/// </summary>
[ApiController]
[Route("cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly GetCartHandler getCartHandler;
    private readonly AddCartItemHandler addCartItemHandler;
    private readonly UpdateCartItemHandler updateCartItemHandler;
    private readonly DeleteCartItemHandler deleteCartItemHandler;
    private readonly IValidator<AddCartItemCommand> addCartItemValidator;
    private readonly IValidator<UpdateCartItemCommand> updateCartItemValidator;
    private readonly ILogger<CartController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CartController"/> class.
    /// </summary>
    /// <param name="getCartHandler">The get cart handler.</param>
    /// <param name="addCartItemHandler">The add cart item handler.</param>
    /// <param name="updateCartItemHandler">The update cart item handler.</param>
    /// <param name="deleteCartItemHandler">The delete cart item handler.</param>
    /// <param name="addCartItemValidator">The add cart item validator.</param>
    /// <param name="updateCartItemValidator">The update cart item validator.</param>
    /// <param name="logger">The logger.</param>
    public CartController(
        GetCartHandler getCartHandler,
        AddCartItemHandler addCartItemHandler,
        UpdateCartItemHandler updateCartItemHandler,
        DeleteCartItemHandler deleteCartItemHandler,
        IValidator<AddCartItemCommand> addCartItemValidator,
        IValidator<UpdateCartItemCommand> updateCartItemValidator,
        ILogger<CartController> logger)
    {
        this.getCartHandler = getCartHandler;
        this.addCartItemHandler = addCartItemHandler;
        this.updateCartItemHandler = updateCartItemHandler;
        this.deleteCartItemHandler = deleteCartItemHandler;
        this.addCartItemValidator = addCartItemValidator;
        this.updateCartItemValidator = updateCartItemValidator;
        this.logger = logger;
    }

    /// <summary>
    /// Get the authenticated user's cart.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cart with items and subtotal.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCart(CancellationToken cancellationToken)
    {
        var userId = this.GetUserId();
        if (!userId.HasValue)
        {
            return this.Unauthorized(new { Message = "Invalid token." });
        }

        var query = new GetCartQuery(userId.Value);
        var result = await this.getCartHandler.Handle(query, cancellationToken);

        return this.Ok(result);
    }

    /// <summary>
    /// Add an item to the cart.
    /// </summary>
    /// <param name="request">The add cart item request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created cart item.</returns>
    [HttpPost("items")]
    [ProducesResponseType(typeof(CartItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddCartItem(
        [FromBody] AddCartItemRequest request,
        CancellationToken cancellationToken)
    {
        var userId = this.GetUserId();
        if (!userId.HasValue)
        {
            return this.Unauthorized(new { Message = "Invalid token." });
        }

        var command = new AddCartItemCommand(
            userId.Value,
            request.ProductId,
            request.VariantId,
            request.Quantity);

        var validationResult = await this.addCartItemValidator.ValidateAsync(command, cancellationToken);
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
            var result = await this.addCartItemHandler.Handle(command, cancellationToken);
            return this.CreatedAtAction(nameof(this.GetCart), null, result);
        }
        catch (InvalidOperationException ex)
        {
            return this.BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Update a cart item (quantity, variant, or selected state).
    /// </summary>
    /// <param name="itemId">The cart item ID.</param>
    /// <param name="request">The update cart item request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated cart item.</returns>
    [HttpPatch("items/{itemId}")]
    [ProducesResponseType(typeof(CartItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCartItem(
        Guid itemId,
        [FromBody] UpdateCartItemRequest request,
        CancellationToken cancellationToken)
    {
        var userId = this.GetUserId();
        if (!userId.HasValue)
        {
            return this.Unauthorized(new { Message = "Invalid token." });
        }

        var command = new UpdateCartItemCommand(
            userId.Value,
            itemId,
            request.Quantity,
            request.VariantId,
            request.Selected);

        var validationResult = await this.updateCartItemValidator.ValidateAsync(command, cancellationToken);
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
            var result = await this.updateCartItemHandler.Handle(command, cancellationToken);
            return this.Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return this.StatusCode(StatusCodes.Status403Forbidden, new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return this.NotFound(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a cart item.
    /// </summary>
    /// <param name="itemId">The cart item ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("items/{itemId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCartItem(
        Guid itemId,
        CancellationToken cancellationToken)
    {
        var userId = this.GetUserId();
        if (!userId.HasValue)
        {
            return this.Unauthorized(new { Message = "Invalid token." });
        }

        var command = new DeleteCartItemCommand(userId.Value, itemId);

        try
        {
            await this.deleteCartItemHandler.Handle(command, cancellationToken);
            return this.NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return this.StatusCode(StatusCodes.Status403Forbidden, new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return this.NotFound(new { Message = ex.Message });
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
