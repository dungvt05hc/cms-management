// <copyright file="ProductsController.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Features.Products;
using Application.Features.Products.GetProductBySlug;
using Application.Features.Products.GetProducts;
using Application.Features.Products.GetProductSuggestions;
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
    private readonly GetProductBySlugHandler getProductBySlugHandler;
    private readonly GetProductSuggestionsHandler getProductSuggestionsHandler;
    private readonly ILogger<ProductsController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductsController"/> class.
    /// </summary>
    /// <param name="getProductsHandler">The get products handler.</param>
    /// <param name="getProductBySlugHandler">The get product by slug handler.</param>
    /// <param name="getProductSuggestionsHandler">The get product suggestions handler.</param>
    /// <param name="logger">The logger.</param>
    public ProductsController(
        GetProductsHandler getProductsHandler,
        GetProductBySlugHandler getProductBySlugHandler,
        GetProductSuggestionsHandler getProductSuggestionsHandler,
        ILogger<ProductsController> logger)
    {
        this.getProductsHandler = getProductsHandler;
        this.getProductBySlugHandler = getProductBySlugHandler;
        this.getProductSuggestionsHandler = getProductSuggestionsHandler;
        this.logger = logger;
    }

    /// <summary>
    /// Get products with optional filters (Public/Anonymous).
    /// Supports both cursor pagination (preferred) and offset pagination (legacy).
    /// </summary>
    /// <param name="category">Optional category filter (slug or id).</param>
    /// <param name="categoryId">Optional category ID filter.</param>
    /// <param name="q">Optional search term.</param>
    /// <param name="featured">Optional featured filter.</param>
    /// <param name="sort">Sort order: priceAsc or priceDesc.</param>
    /// <param name="cursor">Cursor for pagination.</param>
    /// <param name="limit">Page size for cursor pagination (default 20, max 100).</param>
    /// <param name="page">Page number for offset pagination (default 1).</param>
    /// <param name="pageSize">Page size for offset pagination (default 10, max 100).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of products.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(CursorPagedResult<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PagedResult<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts(
        [FromQuery] string? category,
        [FromQuery] Guid? categoryId,
        [FromQuery] string? q,
        [FromQuery] bool? featured,
        [FromQuery] string? sort,
        [FromQuery] string? cursor,
        [FromQuery] int? limit,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken = default)
    {
        // Parse category (could be slug or id)
        Guid? parsedCategoryId = categoryId;
        string? categorySlug = null;

        if (!string.IsNullOrWhiteSpace(category))
        {
            if (Guid.TryParse(category, out var guidValue))
            {
                parsedCategoryId = guidValue;
            }
            else
            {
                categorySlug = category;
            }
        }

        var query = new GetProductsQuery(
            parsedCategoryId,
            categorySlug,
            q,
            featured,
            sort,
            cursor,
            limit,
            page,
            pageSize);

        // Use cursor pagination if cursor or limit are provided
        if (!string.IsNullOrWhiteSpace(cursor) || limit.HasValue)
        {
            var result = await this.getProductsHandler.HandleCursor(query, cancellationToken);
            return this.Ok(result);
        }

        // Fall back to offset pagination for backward compatibility
        var legacyResult = await this.getProductsHandler.Handle(query, cancellationToken);
        return this.Ok(legacyResult);
    }

    /// <summary>
    /// Get product by slug (Public/Anonymous).
    /// </summary>
    /// <param name="slug">The product slug.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product details.</returns>
    [HttpGet("{slug}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductBySlug(
        string slug,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductBySlugQuery(slug);
        var result = await this.getProductBySlugHandler.Handle(query, cancellationToken);

        if (result == null)
        {
            return this.NotFound(new { message = "Product not found" });
        }

        return this.Ok(result);
    }

    /// <summary>
    /// Get product suggestions by slug (same category, Public/Anonymous).
    /// </summary>
    /// <param name="slug">The product slug.</param>
    /// <param name="limit">Maximum number of suggestions (default 4).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of suggested products.</returns>
    [HttpGet("{slug}/suggestions")]
    [ProducesResponseType(typeof(List<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductSuggestions(
        string slug,
        [FromQuery] int limit = 4,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductSuggestionsQuery(slug, limit);
        var result = await this.getProductSuggestionsHandler.Handle(query, cancellationToken);

        return this.Ok(result);
    }
}
