// <copyright file="CategoriesController.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Features.Categories.GetCategoryTree;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Public categories controller.
/// </summary>
[ApiController]
[Route("categories")]
public class CategoriesController : ControllerBase
{
    private readonly GetCategoryTreeHandler getCategoryTreeHandler;
    private readonly ILogger<CategoriesController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CategoriesController"/> class.
    /// </summary>
    /// <param name="getCategoryTreeHandler">The get category tree handler.</param>
    /// <param name="logger">The logger.</param>
    public CategoriesController(
        GetCategoryTreeHandler getCategoryTreeHandler,
        ILogger<CategoriesController> logger)
    {
        this.getCategoryTreeHandler = getCategoryTreeHandler;
        this.logger = logger;
    }

    /// <summary>
    /// Get the category tree (Anonymous/Public).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The category tree.</returns>
    [HttpGet("tree")]
    [ProducesResponseType(typeof(List<CategoryTreeNode>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategoryTree(CancellationToken cancellationToken)
    {
        var query = new GetCategoryTreeQuery();
        var result = await this.getCategoryTreeHandler.Handle(query, cancellationToken);
        return this.Ok(result);
    }
}
