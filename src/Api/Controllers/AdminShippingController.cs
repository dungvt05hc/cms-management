// <copyright file="AdminShippingController.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Features.Shipping.UpdateShippingConfig;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Admin shipping controller.
/// </summary>
[ApiController]
[Route("admin/shipping")]
[Authorize(Policy = "AdminOnly")]
public class AdminShippingController : ControllerBase
{
    private readonly UpdateShippingConfigHandler updateShippingConfigHandler;
    private readonly IValidator<UpdateShippingConfigCommand> updateShippingConfigValidator;
    private readonly ILogger<AdminShippingController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminShippingController"/> class.
    /// </summary>
    /// <param name="updateShippingConfigHandler">The update shipping config handler.</param>
    /// <param name="updateShippingConfigValidator">The update shipping config validator.</param>
    /// <param name="logger">The logger.</param>
    public AdminShippingController(
        UpdateShippingConfigHandler updateShippingConfigHandler,
        IValidator<UpdateShippingConfigCommand> updateShippingConfigValidator,
        ILogger<AdminShippingController> logger)
    {
        this.updateShippingConfigHandler = updateShippingConfigHandler;
        this.updateShippingConfigValidator = updateShippingConfigValidator;
        this.logger = logger;
    }

    /// <summary>
    /// Update shipping configuration (Admin only).
    /// </summary>
    /// <param name="command">The update command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success result.</returns>
    [HttpPut("config")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateShippingConfig(
        [FromBody] UpdateShippingConfigCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await this.updateShippingConfigValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return this.BadRequest(new
            {
                Message = "Validation failed.",
                Errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }),
            });
        }

        await this.updateShippingConfigHandler.Handle(command, cancellationToken);

        this.logger.LogInformation("Shipping configuration updated by admin");

        return this.Ok(new { Message = "Shipping configuration updated successfully." });
    }
}
