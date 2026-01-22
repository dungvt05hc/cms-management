// <copyright file="AddressesController.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Security.Claims;

using Application.Features.Addresses;
using Application.Features.Addresses.CreateAddress;
using Application.Features.Addresses.DeleteAddress;
using Application.Features.Addresses.GetAddresses;
using Application.Features.Addresses.SetDefaultAddress;
using Application.Features.Addresses.UpdateAddress;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Addresses controller for authenticated buyers.
/// </summary>
[ApiController]
[Route("me/addresses")]
[Authorize]
public class AddressesController : ControllerBase
{
    private readonly GetAddressesHandler getAddressesHandler;
    private readonly CreateAddressHandler createAddressHandler;
    private readonly UpdateAddressHandler updateAddressHandler;
    private readonly DeleteAddressHandler deleteAddressHandler;
    private readonly SetDefaultAddressHandler setDefaultAddressHandler;
    private readonly IValidator<CreateAddressCommand> createAddressValidator;
    private readonly IValidator<UpdateAddressCommand> updateAddressValidator;
    private readonly ILogger<AddressesController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddressesController"/> class.
    /// </summary>
    /// <param name="getAddressesHandler">The get addresses handler.</param>
    /// <param name="createAddressHandler">The create address handler.</param>
    /// <param name="updateAddressHandler">The update address handler.</param>
    /// <param name="deleteAddressHandler">The delete address handler.</param>
    /// <param name="setDefaultAddressHandler">The set default address handler.</param>
    /// <param name="createAddressValidator">The create address validator.</param>
    /// <param name="updateAddressValidator">The update address validator.</param>
    /// <param name="logger">The logger.</param>
    public AddressesController(
        GetAddressesHandler getAddressesHandler,
        CreateAddressHandler createAddressHandler,
        UpdateAddressHandler updateAddressHandler,
        DeleteAddressHandler deleteAddressHandler,
        SetDefaultAddressHandler setDefaultAddressHandler,
        IValidator<CreateAddressCommand> createAddressValidator,
        IValidator<UpdateAddressCommand> updateAddressValidator,
        ILogger<AddressesController> logger)
    {
        this.getAddressesHandler = getAddressesHandler;
        this.createAddressHandler = createAddressHandler;
        this.updateAddressHandler = updateAddressHandler;
        this.deleteAddressHandler = deleteAddressHandler;
        this.setDefaultAddressHandler = setDefaultAddressHandler;
        this.createAddressValidator = createAddressValidator;
        this.updateAddressValidator = updateAddressValidator;
        this.logger = logger;
    }

    /// <summary>
    /// Get all addresses for the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of addresses.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<AddressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAddresses(CancellationToken cancellationToken)
    {
        var userId = this.GetUserId();
        if (!userId.HasValue)
        {
            return this.Unauthorized(new { Message = "Invalid token." });
        }

        var query = new GetAddressesQuery(userId.Value);
        var result = await this.getAddressesHandler.Handle(query, cancellationToken);

        return this.Ok(result);
    }

    /// <summary>
    /// Create a new address for the authenticated user.
    /// </summary>
    /// <param name="request">The create address request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created address.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateAddress(
        [FromBody] CreateAddressRequest request,
        CancellationToken cancellationToken)
    {
        var userId = this.GetUserId();
        if (!userId.HasValue)
        {
            return this.Unauthorized(new { Message = "Invalid token." });
        }

        var command = new CreateAddressCommand(
            userId.Value,
            request.FullName,
            request.Phone,
            request.AddressLine,
            request.Ward,
            request.District,
            request.City,
            request.IsDefault);

        var validationResult = await this.createAddressValidator.ValidateAsync(command, cancellationToken);
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
            var result = await this.createAddressHandler.Handle(command, cancellationToken);
            return this.CreatedAtAction(nameof(this.GetAddresses), null, result);
        }
        catch (InvalidOperationException ex)
        {
            return this.BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing address.
    /// </summary>
    /// <param name="id">The address ID.</param>
    /// <param name="request">The update address request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated address.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAddress(
        Guid id,
        [FromBody] UpdateAddressRequest request,
        CancellationToken cancellationToken)
    {
        var userId = this.GetUserId();
        if (!userId.HasValue)
        {
            return this.Unauthorized(new { Message = "Invalid token." });
        }

        var command = new UpdateAddressCommand(
            id,
            userId.Value,
            request.FullName,
            request.Phone,
            request.AddressLine,
            request.Ward,
            request.District,
            request.City);

        var validationResult = await this.updateAddressValidator.ValidateAsync(command, cancellationToken);
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
            var result = await this.updateAddressHandler.Handle(command, cancellationToken);
            return this.Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return this.NotFound(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Delete an address.
    /// </summary>
    /// <param name="id">The address ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAddress(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = this.GetUserId();
        if (!userId.HasValue)
        {
            return this.Unauthorized(new { Message = "Invalid token." });
        }

        var command = new DeleteAddressCommand(id, userId.Value);

        try
        {
            await this.deleteAddressHandler.Handle(command, cancellationToken);
            return this.NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return this.NotFound(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Set an address as default.
    /// </summary>
    /// <param name="id">The address ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated address.</returns>
    [HttpPut("{id}/default")]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetDefaultAddress(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = this.GetUserId();
        if (!userId.HasValue)
        {
            return this.Unauthorized(new { Message = "Invalid token." });
        }

        var command = new SetDefaultAddressCommand(id, userId.Value);

        try
        {
            var result = await this.setDefaultAddressHandler.Handle(command, cancellationToken);
            return this.Ok(result);
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
