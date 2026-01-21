// <copyright file="ProductsController.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Features.Products;
using Application.Features.Products.GetProducts;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Public products controller for storefront.
/// </summary>
[ApiController]
[Route("products")]
public class ProductsController : ControllerBase
{
    private readonly GetProductsHandler getProductsHandler;
    private readonly ILogger<ProductsController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductsController"/> class.
    /// </summary>
    /// <param name="getProductsHandler">The get products handler.</param>
    /// <param name="logger">The logger.</param>
    public ProductsController(
        GetProductsHandler getProductsHandler,
        ILogger<ProductsController> logger)
    {
        this.getProductsHandler = getProductsHandler;
        this.logger = logger;
    }

    /// <summary>
    /// Get products with optional filters (Public/Anonymous).
    /// </summary>
    /// <param name="categoryId">Optional category filter.</param>
    /// <param name="q">Optional search term.</param>
    /// <param name="featured">Optional featured filter.</param>
    /// <param name="page">Page number (default 1).</param>
    /// <param name="pageSize">Page size (default 10, max 100).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of products.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts(
        [FromQuery] Guid? categoryId,
        [FromQuery] string? q,
        [FromQuery] bool? featured,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductsQuery(categoryId, q, featured, page, pageSize);
        var result = await this.getProductsHandler.Handle(query, cancellationToken);
        return this.Ok(result);
    }
}
