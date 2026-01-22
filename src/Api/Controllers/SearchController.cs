// <copyright file="SearchController.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Features.Search;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Search controller for storefront.
/// </summary>
[ApiController]
[Route("search")]
public class SearchController : ControllerBase
{
    private readonly SearchSuggestHandler searchSuggestHandler;
    private readonly ILogger<SearchController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchController"/> class.
    /// </summary>
    /// <param name="searchSuggestHandler">The search suggest handler.</param>
    /// <param name="logger">The logger.</param>
    public SearchController(
        SearchSuggestHandler searchSuggestHandler,
        ILogger<SearchController> logger)
    {
        this.searchSuggestHandler = searchSuggestHandler;
        this.logger = logger;
    }

    /// <summary>
    /// Get search suggestions (Public/Anonymous).
    /// Returns product suggestions based on the search query.
    /// </summary>
    /// <param name="q">The search query (minimum 2 characters).</param>
    /// <param name="limit">Maximum number of suggestions (default 10, max 20).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of search suggestions.</returns>
    [HttpGet("suggest")]
    [ProducesResponseType(typeof(List<SearchSuggestionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSuggestions(
        [FromQuery] string? q,
        [FromQuery] int? limit,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchSuggestQuery(q, limit);
        var suggestions = await this.searchSuggestHandler.Handle(query, cancellationToken);

        return this.Ok(suggestions);
    }
}
