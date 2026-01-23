// <copyright file="CheckoutController.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Security.Claims;

using Application.Features.Checkout;
using Application.Features.Checkout.ApplyVoucher;
using Application.Features.Checkout.Preview;
using Application.Features.Checkout.Submit;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Checkout controller for authenticated buyers.
/// </summary>
[ApiController]
[Route("checkout")]
[Authorize]
public class CheckoutController : ControllerBase
{
    private readonly ApplyVoucherHandler applyVoucherHandler;
    private readonly IValidator<ApplyVoucherCommand> applyVoucherValidator;
    private readonly CheckoutPreviewHandler checkoutPreviewHandler;
    private readonly IValidator<CheckoutPreviewCommand> checkoutPreviewValidator;
    private readonly CheckoutSubmitHandler checkoutSubmitHandler;
    private readonly IValidator<CheckoutSubmitCommand> checkoutSubmitValidator;
    private readonly ILogger<CheckoutController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckoutController"/> class.
    /// </summary>
    /// <param name="applyVoucherHandler">The apply voucher handler.</param>
    /// <param name="applyVoucherValidator">The apply voucher validator.</param>
    /// <param name="checkoutPreviewHandler">The checkout preview handler.</param>
    /// <param name="checkoutPreviewValidator">The checkout preview validator.</param>
    /// <param name="checkoutSubmitHandler">The checkout submit handler.</param>
    /// <param name="checkoutSubmitValidator">The checkout submit validator.</param>
    /// <param name="logger">The logger.</param>
    public CheckoutController(
        ApplyVoucherHandler applyVoucherHandler,
        IValidator<ApplyVoucherCommand> applyVoucherValidator,
        CheckoutPreviewHandler checkoutPreviewHandler,
        IValidator<CheckoutPreviewCommand> checkoutPreviewValidator,
        CheckoutSubmitHandler checkoutSubmitHandler,
        IValidator<CheckoutSubmitCommand> checkoutSubmitValidator,
        ILogger<CheckoutController> logger)
    {
        this.applyVoucherHandler = applyVoucherHandler;
        this.applyVoucherValidator = applyVoucherValidator;
        this.checkoutPreviewHandler = checkoutPreviewHandler;
        this.checkoutPreviewValidator = checkoutPreviewValidator;
        this.checkoutSubmitHandler = checkoutSubmitHandler;
        this.checkoutSubmitValidator = checkoutSubmitValidator;
        this.logger = logger;
    }

    /// <summary>
    /// Apply discount and/or shipping vouchers and return checkout totals.
    /// </summary>
    /// <param name="request">The apply voucher request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The checkout totals with applied vouchers.</returns>
    [HttpPost("apply-voucher")]
    [ProducesResponseType(typeof(CheckoutTotalsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ApplyVoucher(
        [FromBody] ApplyVoucherRequest request,
        CancellationToken cancellationToken)
    {
        var userId = this.GetUserId();
        if (!userId.HasValue)
        {
            return this.Unauthorized(new { Message = "Invalid token." });
        }

        var command = new ApplyVoucherCommand(
            userId.Value,
            request.DiscountCode,
            request.ShippingCode);

        var validationResult = await this.applyVoucherValidator.ValidateAsync(command, cancellationToken);
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
            var result = await this.applyVoucherHandler.Handle(command, cancellationToken);
            return this.Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return this.BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Preview checkout totals with address and shipping.
    /// </summary>
    /// <param name="request">The checkout preview request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The checkout totals.</returns>
    [HttpPost("preview")]
    [ProducesResponseType(typeof(CheckoutTotalsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Preview(
        [FromBody] CheckoutPreviewRequest request,
        CancellationToken cancellationToken)
    {
        var userId = this.GetUserId();
        if (!userId.HasValue)
        {
            return this.Unauthorized(new { Message = "Invalid token." });
        }

        var command = new CheckoutPreviewCommand(
            userId.Value,
            request.AddressId,
            request.ShippingMethodCode,
            request.ShippingCarrierCode,
            request.DiscountCode,
            request.ShippingCode);

        var validationResult = await this.checkoutPreviewValidator.ValidateAsync(command, cancellationToken);
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
            var result = await this.checkoutPreviewHandler.Handle(command, cancellationToken);
            return this.Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return this.BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Submit checkout and create order (COD) or initiate payment (Payoo).
    /// </summary>
    /// <param name="request">The checkout submit request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The checkout submit result.</returns>
    [HttpPost("submit")]
    [ProducesResponseType(typeof(CheckoutSubmitResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Submit(
        [FromBody] CheckoutSubmitRequest request,
        CancellationToken cancellationToken)
    {
        var userId = this.GetUserId();
        if (!userId.HasValue)
        {
            return this.Unauthorized(new { Message = "Invalid token." });
        }

        var command = new CheckoutSubmitCommand(
            userId.Value,
            request.AddressId,
            request.ShippingMethodCode,
            request.ShippingCarrierCode,
            request.DiscountCode,
            request.ShippingCode,
            request.PaymentMethod,
            request.Notes);

        var validationResult = await this.checkoutSubmitValidator.ValidateAsync(command, cancellationToken);
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
            var result = await this.checkoutSubmitHandler.Handle(command, cancellationToken);
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
