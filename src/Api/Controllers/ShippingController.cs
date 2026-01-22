// <copyright file="ShippingController.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Security.Claims;

using Application.Features.Shipping.GetShippingMethods;
using Application.Features.Shipping.GetShippingQuote;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Shipping controller for public and authenticated buyers.
/// </summary>
[ApiController]
[Route("shipping")]
public class ShippingController : ControllerBase
{
    private readonly GetShippingMethodsHandler getShippingMethodsHandler;
    private readonly GetShippingQuoteHandler getShippingQuoteHandler;
    private readonly IValidator<GetShippingQuoteQuery> getShippingQuoteValidator;
    private readonly ILogger<ShippingController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShippingController"/> class.
    /// </summary>
    /// <param name="getShippingMethodsHandler">The get shipping methods handler.</param>
    /// <param name="getShippingQuoteHandler">The get shipping quote handler.</param>
    /// <param name="getShippingQuoteValidator">The get shipping quote validator.</param>
    /// <param name="logger">The logger.</param>
    public ShippingController(
        GetShippingMethodsHandler getShippingMethodsHandler,
        GetShippingQuoteHandler getShippingQuoteHandler,
        IValidator<GetShippingQuoteQuery> getShippingQuoteValidator,
        ILogger<ShippingController> logger)
    {
        this.getShippingMethodsHandler = getShippingMethodsHandler;
        this.getShippingQuoteHandler = getShippingQuoteHandler;
        this.getShippingQuoteValidator = getShippingQuoteValidator;
        this.logger = logger;
    }

    /// <summary>
    /// Get all active shipping methods (anonymous).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of shipping methods.</returns>
    [HttpGet("methods")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<Application.Features.Shipping.ShippingMethodDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetShippingMethods(CancellationToken cancellationToken)
    {
        var query = new GetShippingMethodsQuery();
        var result = await this.getShippingMethodsHandler.Handle(query, cancellationToken);

        return this.Ok(result);
    }

    /// <summary>
    /// Get a shipping quote for a specific address and method/carrier.
    /// </summary>
    /// <param name="request">The quote request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Shipping quote with fee and ETA.</returns>
    [HttpPost("quote")]
    [Authorize]
    [ProducesResponseType(typeof(Application.Features.Shipping.GetShippingQuote.ShippingQuoteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetShippingQuote(
        [FromBody] ShippingQuoteRequest request,
        CancellationToken cancellationToken)
    {
        var userId = this.GetUserId();
        if (!userId.HasValue)
        {
            return this.Unauthorized(new { Message = "Invalid token." });
        }

        var query = new GetShippingQuoteQuery(
            userId.Value,
            request.AddressId,
            request.MethodCode,
            request.CarrierCode,
            request.Weight);

        var validationResult = await this.getShippingQuoteValidator.ValidateAsync(query, cancellationToken);
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
            var result = await this.getShippingQuoteHandler.Handle(query, cancellationToken);
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
