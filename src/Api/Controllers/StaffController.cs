// <copyright file="StaffController.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Features.Staff.CreateStaff;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Staff management controller.
/// </summary>
[ApiController]
[Route("admin/staff")]
[Authorize(Policy = "AdminOnly")]
public class StaffController : ControllerBase
{
    private readonly CreateStaffHandler createStaffHandler;
    private readonly IValidator<CreateStaffCommand> createStaffValidator;
    private readonly ILogger<StaffController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="StaffController"/> class.
    /// </summary>
    /// <param name="createStaffHandler">The create staff handler.</param>
    /// <param name="createStaffValidator">The create staff validator.</param>
    /// <param name="logger">The logger.</param>
    public StaffController(
        CreateStaffHandler createStaffHandler,
        IValidator<CreateStaffCommand> createStaffValidator,
        ILogger<StaffController> logger)
    {
        this.createStaffHandler = createStaffHandler;
        this.createStaffValidator = createStaffValidator;
        this.logger = logger;
    }

    /// <summary>
    /// Create a new staff user (Admin only).
    /// </summary>
    /// <param name="command">The create staff command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created staff user details.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateStaffResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateStaff(
        [FromBody] CreateStaffCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await this.createStaffValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return this.BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        try
        {
            var result = await this.createStaffHandler.Handle(command, cancellationToken);
            return this.CreatedAtAction(nameof(this.CreateStaff), new { id = result.StaffId }, result);
        }
        catch (InvalidOperationException ex)
        {
            return this.Conflict(new { Message = ex.Message });
        }
    }
}
