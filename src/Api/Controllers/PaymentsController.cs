// <copyright file="PaymentsController.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Features.Payments.PayooCallback;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Controller for payment callbacks.
/// </summary>
[ApiController]
[Route("payments")]
public class PaymentsController : ControllerBase
{
    private readonly PayooCallbackHandler payooCallbackHandler;
    private readonly IValidator<PayooCallbackCommand> payooCallbackValidator;
    private readonly ILogger<PaymentsController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentsController"/> class.
    /// </summary>
    /// <param name="payooCallbackHandler">The Payoo callback handler.</param>
    /// <param name="payooCallbackValidator">The Payoo callback validator.</param>
    /// <param name="logger">The logger.</param>
    public PaymentsController(
        PayooCallbackHandler payooCallbackHandler,
        IValidator<PayooCallbackCommand> payooCallbackValidator,
        ILogger<PaymentsController> logger)
    {
        this.payooCallbackHandler = payooCallbackHandler;
        this.payooCallbackValidator = payooCallbackValidator;
        this.logger = logger;
    }

    /// <summary>
    /// Handles Payoo payment callback (idempotent).
    /// </summary>
    /// <param name="request">The callback request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The callback result.</returns>
    [HttpPost("payoo/callback")]
    [ProducesResponseType(typeof(PayooCallbackResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PayooCallback(
        [FromBody] PayooCallbackRequest request,
        CancellationToken cancellationToken)
    {
        var command = new PayooCallbackCommand(
            request.PaymentReference,
            request.UserId,
            request.AddressId,
            request.ShippingMethodCode,
            request.ShippingCarrierCode,
            request.DiscountCode,
            request.ShippingCode,
            request.Notes);

        var validationResult = await this.payooCallbackValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return this.BadRequest(new
            {
                Message = "Validation failed.",
                Errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }),
            });
        }

        var result = await this.payooCallbackHandler.Handle(command, cancellationToken);

        if (!result.Success)
        {
            return this.BadRequest(new { Message = result.ErrorMessage });
        }

        return this.Ok(result);
    }
}
