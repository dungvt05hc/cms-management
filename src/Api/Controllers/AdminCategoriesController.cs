// <copyright file="AdminCategoriesController.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Features.Categories.CreateCategory;
using Application.Features.Categories.DeleteCategory;
using Application.Features.Categories.UpdateCategory;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Admin categories controller.
/// </summary>
[ApiController]
[Route("admin/categories")]
[Authorize(Policy = "AdminOnly")]
public class AdminCategoriesController : ControllerBase
{
    private readonly CreateCategoryHandler createCategoryHandler;
    private readonly UpdateCategoryHandler updateCategoryHandler;
    private readonly DeleteCategoryHandler deleteCategoryHandler;
    private readonly IValidator<CreateCategoryCommand> createCategoryValidator;
    private readonly IValidator<UpdateCategoryCommand> updateCategoryValidator;
    private readonly ILogger<AdminCategoriesController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminCategoriesController"/> class.
    /// </summary>
    /// <param name="createCategoryHandler">The create category handler.</param>
    /// <param name="updateCategoryHandler">The update category handler.</param>
    /// <param name="deleteCategoryHandler">The delete category handler.</param>
    /// <param name="createCategoryValidator">The create category validator.</param>
    /// <param name="updateCategoryValidator">The update category validator.</param>
    /// <param name="logger">The logger.</param>
    public AdminCategoriesController(
        CreateCategoryHandler createCategoryHandler,
        UpdateCategoryHandler updateCategoryHandler,
        DeleteCategoryHandler deleteCategoryHandler,
        IValidator<CreateCategoryCommand> createCategoryValidator,
        IValidator<UpdateCategoryCommand> updateCategoryValidator,
        ILogger<AdminCategoriesController> logger)
    {
        this.createCategoryHandler = createCategoryHandler;
        this.updateCategoryHandler = updateCategoryHandler;
        this.deleteCategoryHandler = deleteCategoryHandler;
        this.createCategoryValidator = createCategoryValidator;
        this.updateCategoryValidator = updateCategoryValidator;
        this.logger = logger;
    }

    /// <summary>
    /// Create a new category (Admin only).
    /// </summary>
    /// <param name="command">The create category command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created category.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateCategory(
        [FromBody] CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await this.createCategoryValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return this.BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        try
        {
            var result = await this.createCategoryHandler.Handle(command, cancellationToken);
            return this.CreatedAtAction(nameof(this.CreateCategory), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return this.Conflict(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Update a category (Admin only).
    /// </summary>
    /// <param name="id">The category identifier.</param>
    /// <param name="request">The update category request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateCategory(
        Guid id,
        [FromBody] UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCategoryCommand(id, request.Name, request.ParentId);
        var validationResult = await this.updateCategoryValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return this.BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        try
        {
            await this.updateCategoryHandler.Handle(command, cancellationToken);
            return this.Ok(new { Message = "Category updated successfully." });
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("not found"))
            {
                return this.NotFound(new { Message = ex.Message });
            }

            return this.Conflict(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a category (Admin only).
    /// </summary>
    /// <param name="id">The category identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteCategoryCommand(id);

        try
        {
            await this.deleteCategoryHandler.Handle(command, cancellationToken);
            return this.NoContent();
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("not found"))
            {
                return this.NotFound(new { Message = ex.Message });
            }

            return this.Conflict(new { Message = ex.Message });
        }
    }
}
