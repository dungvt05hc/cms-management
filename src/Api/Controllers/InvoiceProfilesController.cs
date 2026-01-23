// <copyright file="InvoiceProfilesController.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Security.Claims;

using Application.Features.InvoiceProfiles;
using Application.Features.InvoiceProfiles.CreateInvoiceProfile;
using Application.Features.InvoiceProfiles.GetInvoiceProfiles;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Invoice profiles controller for authenticated buyers.
/// </summary>
[ApiController]
[Route("me/invoice-profiles")]
[Authorize]
public class InvoiceProfilesController : ControllerBase
{
    private readonly GetInvoiceProfilesHandler getInvoiceProfilesHandler;
    private readonly CreateInvoiceProfileHandler createInvoiceProfileHandler;
    private readonly IValidator<CreateInvoiceProfileCommand> createInvoiceProfileValidator;
    private readonly ILogger<InvoiceProfilesController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="InvoiceProfilesController"/> class.
    /// </summary>
    /// <param name="getInvoiceProfilesHandler">The get invoice profiles handler.</param>
    /// <param name="createInvoiceProfileHandler">The create invoice profile handler.</param>
    /// <param name="createInvoiceProfileValidator">The create invoice profile validator.</param>
    /// <param name="logger">The logger.</param>
    public InvoiceProfilesController(
        GetInvoiceProfilesHandler getInvoiceProfilesHandler,
        CreateInvoiceProfileHandler createInvoiceProfileHandler,
        IValidator<CreateInvoiceProfileCommand> createInvoiceProfileValidator,
        ILogger<InvoiceProfilesController> logger)
    {
        this.getInvoiceProfilesHandler = getInvoiceProfilesHandler;
        this.createInvoiceProfileHandler = createInvoiceProfileHandler;
        this.createInvoiceProfileValidator = createInvoiceProfileValidator;
        this.logger = logger;
    }

    /// <summary>
    /// Get all invoice profiles for the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of invoice profiles.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<InvoiceProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetInvoiceProfiles(CancellationToken cancellationToken)
    {
        var userId = this.GetUserId();
        if (!userId.HasValue)
        {
            return this.Unauthorized(new { Message = "Invalid token." });
        }

        var query = new GetInvoiceProfilesQuery(userId.Value);
        var result = await this.getInvoiceProfilesHandler.Handle(query, cancellationToken);

        return this.Ok(result);
    }

    /// <summary>
    /// Create a new invoice profile for the authenticated user.
    /// </summary>
    /// <param name="request">The create invoice profile request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created invoice profile.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(InvoiceProfileDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateInvoiceProfile(
        [FromBody] CreateInvoiceProfileRequest request,
        CancellationToken cancellationToken)
    {
        var userId = this.GetUserId();
        if (!userId.HasValue)
        {
            return this.Unauthorized(new { Message = "Invalid token." });
        }

        var command = new CreateInvoiceProfileCommand(
            userId.Value,
            request.TaxCode,
            request.CompanyName,
            request.CompanyAddress,
            request.Email);

        var validationResult = await this.createInvoiceProfileValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return this.BadRequest(new
            {
                Message = "Validation failed.",
                Errors = validationResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage,
                }),
            });
        }

        var result = await this.createInvoiceProfileHandler.Handle(command, cancellationToken);

        return this.CreatedAtAction(
            nameof(this.GetInvoiceProfiles),
            new { id = result.Id },
            result);
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
