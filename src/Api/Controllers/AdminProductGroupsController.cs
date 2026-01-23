// <copyright file="AdminProductGroupsController.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Features.ProductGroups.CreateProductGroup;
using Application.Features.ProductGroups.DeleteProductGroup;
using Application.Features.ProductGroups.GetProductGroups;
using Application.Features.ProductGroups.UpdateProductGroup;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Admin product groups controller.
/// </summary>
[ApiController]
[Route("admin/product-groups")]
[Authorize(Policy = "AdminOnly")]
public class AdminProductGroupsController : ControllerBase
{
    private readonly CreateProductGroupHandler createProductGroupHandler;
    private readonly GetProductGroupsHandler getProductGroupsHandler;
    private readonly UpdateProductGroupHandler updateProductGroupHandler;
    private readonly DeleteProductGroupHandler deleteProductGroupHandler;
    private readonly IValidator<CreateProductGroupCommand> createProductGroupValidator;
    private readonly IValidator<UpdateProductGroupCommand> updateProductGroupValidator;
    private readonly ILogger<AdminProductGroupsController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminProductGroupsController"/> class.
    /// </summary>
    /// <param name="createProductGroupHandler">The create product group handler.</param>
    /// <param name="getProductGroupsHandler">The get product groups handler.</param>
    /// <param name="updateProductGroupHandler">The update product group handler.</param>
    /// <param name="deleteProductGroupHandler">The delete product group handler.</param>
    /// <param name="createProductGroupValidator">The create product group validator.</param>
    /// <param name="updateProductGroupValidator">The update product group validator.</param>
    /// <param name="logger">The logger.</param>
    public AdminProductGroupsController(
        CreateProductGroupHandler createProductGroupHandler,
        GetProductGroupsHandler getProductGroupsHandler,
        UpdateProductGroupHandler updateProductGroupHandler,
        DeleteProductGroupHandler deleteProductGroupHandler,
        IValidator<CreateProductGroupCommand> createProductGroupValidator,
        IValidator<UpdateProductGroupCommand> updateProductGroupValidator,
        ILogger<AdminProductGroupsController> logger)
    {
        this.createProductGroupHandler = createProductGroupHandler;
        this.getProductGroupsHandler = getProductGroupsHandler;
        this.updateProductGroupHandler = updateProductGroupHandler;
        this.deleteProductGroupHandler = deleteProductGroupHandler;
        this.createProductGroupValidator = createProductGroupValidator;
        this.updateProductGroupValidator = updateProductGroupValidator;
        this.logger = logger;
    }

    /// <summary>
    /// Create a new product group (Admin only).
    /// </summary>
    /// <param name="command">The create product group command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created product group.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ProductGroupDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateProductGroup(
        [FromBody] CreateProductGroupCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await this.createProductGroupValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return this.BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        try
        {
            var result = await this.createProductGroupHandler.Handle(command, cancellationToken);
            return this.CreatedAtAction(nameof(this.CreateProductGroup), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return this.BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Get product groups filtered by category (Admin only).
    /// </summary>
    /// <param name="categoryId">Optional category filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of product groups.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<ProductGroupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetProductGroups(
        [FromQuery] Guid? categoryId,
        CancellationToken cancellationToken)
    {
        var query = new GetProductGroupsQuery(categoryId);
        var result = await this.getProductGroupsHandler.Handle(query, cancellationToken);
        return this.Ok(result);
    }

    /// <summary>
    /// Update a product group (Admin only).
    /// </summary>
    /// <param name="id">The product group identifier.</param>
    /// <param name="request">The update product group request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProductGroup(
        Guid id,
        [FromBody] UpdateProductGroupRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductGroupCommand(id, request.CategoryId, request.Name, request.Attributes);
        var validationResult = await this.updateProductGroupValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return this.BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        try
        {
            await this.updateProductGroupHandler.Handle(command, cancellationToken);
            return this.Ok(new { Message = "Product group updated successfully." });
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("not found"))
            {
                return this.NotFound(new { Message = ex.Message });
            }

            return this.BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a product group (Admin only).
    /// </summary>
    /// <param name="id">The product group identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProductGroup(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteProductGroupCommand(id);

        try
        {
            await this.deleteProductGroupHandler.Handle(command, cancellationToken);
            return this.NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return this.NotFound(new { Message = ex.Message });
        }
    }
}
