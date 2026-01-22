// <copyright file="AdminProductsController.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Features.Products;
using Application.Features.Products.CreateProduct;
using Application.Features.Products.DeleteProduct;
using Application.Features.Products.GetProductById;
using Application.Features.Products.GetProducts;
using Application.Features.Products.UpdateProduct;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Admin products controller.
/// </summary>
[ApiController]
[Route("admin/products")]
[Authorize(Policy = "AdminOnly")]
public class AdminProductsController : ControllerBase
{
    private readonly CreateProductHandler createProductHandler;
    private readonly GetProductsHandler getProductsHandler;
    private readonly GetProductByIdHandler getProductByIdHandler;
    private readonly UpdateProductHandler updateProductHandler;
    private readonly DeleteProductHandler deleteProductHandler;
    private readonly IValidator<CreateProductCommand> createProductValidator;
    private readonly IValidator<UpdateProductCommand> updateProductValidator;
    private readonly ILogger<AdminProductsController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminProductsController"/> class.
    /// </summary>
    /// <param name="createProductHandler">The create product handler.</param>
    /// <param name="getProductsHandler">The get products handler.</param>
    /// <param name="getProductByIdHandler">The get product by ID handler.</param>
    /// <param name="updateProductHandler">The update product handler.</param>
    /// <param name="deleteProductHandler">The delete product handler.</param>
    /// <param name="createProductValidator">The create product validator.</param>
    /// <param name="updateProductValidator">The update product validator.</param>
    /// <param name="logger">The logger.</param>
    public AdminProductsController(
        CreateProductHandler createProductHandler,
        GetProductsHandler getProductsHandler,
        GetProductByIdHandler getProductByIdHandler,
        UpdateProductHandler updateProductHandler,
        DeleteProductHandler deleteProductHandler,
        IValidator<CreateProductCommand> createProductValidator,
        IValidator<UpdateProductCommand> updateProductValidator,
        ILogger<AdminProductsController> logger)
    {
        this.createProductHandler = createProductHandler;
        this.getProductsHandler = getProductsHandler;
        this.getProductByIdHandler = getProductByIdHandler;
        this.updateProductHandler = updateProductHandler;
        this.deleteProductHandler = deleteProductHandler;
        this.createProductValidator = createProductValidator;
        this.updateProductValidator = updateProductValidator;
        this.logger = logger;
    }

    /// <summary>
    /// Create a new product (Admin only).
    /// </summary>
    /// <param name="command">The create product command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created product.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateProduct(
        [FromBody] CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await this.createProductValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return this.BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        try
        {
            var result = await this.createProductHandler.Handle(command, cancellationToken);
            this.logger.LogInformation("Product created: {ProductId}", result.Id);
            return this.CreatedAtAction(nameof(this.GetProductById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return this.Conflict(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Get products with filters (Admin only).
    /// </summary>
    /// <param name="categoryId">Filter by category.</param>
    /// <param name="q">Search term.</param>
    /// <param name="featured">Filter by featured.</param>
    /// <param name="page">Page number.</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of products.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetProducts(
        [FromQuery] Guid? categoryId,
        [FromQuery] string? q,
        [FromQuery] bool? featured,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductsQuery(categoryId, null, q, featured, null, null, null, page, pageSize);
        var result = await this.getProductsHandler.Handle(query, cancellationToken);
        return this.Ok(result);
    }

    /// <summary>
    /// Get product by ID (Admin only).
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery(id);

        try
        {
            var result = await this.getProductByIdHandler.Handle(query, cancellationToken);
            return this.Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return this.NotFound(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Update a product (Admin only).
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="request">The update product request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateProduct(
        Guid id,
        [FromBody] UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand(
            id,
            request.Name,
            request.Slug,
            request.Description,
            request.CategoryId,
            request.Images,
            request.Videos,
            request.Specifications,
            request.IsActive,
            request.IsFeatured,
            request.Variants);

        var validationResult = await this.updateProductValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return this.BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        try
        {
            await this.updateProductHandler.Handle(command, cancellationToken);
            this.logger.LogInformation("Product updated: {ProductId}", id);
            return this.Ok(new { Message = "Product updated successfully." });
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
    /// Delete a product (Admin only).
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteProductCommand(id);

        try
        {
            await this.deleteProductHandler.Handle(command, cancellationToken);
            this.logger.LogInformation("Product deleted: {ProductId}", id);
            return this.NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return this.NotFound(new { Message = ex.Message });
        }
    }
}
