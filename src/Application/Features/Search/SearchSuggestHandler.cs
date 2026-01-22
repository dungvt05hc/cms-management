// <copyright file="SearchSuggestHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Search;

/// <summary>
/// Handler for the SearchSuggestQuery.
/// </summary>
public class SearchSuggestHandler
{
    private const int DefaultLimit = 10;
    private const int MaxLimit = 20;
    private const int MinQueryLength = 2;

    private readonly IAppDbContext dbContext;
    private readonly ILogger<SearchSuggestHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchSuggestHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger.</param>
    public SearchSuggestHandler(
        IAppDbContext dbContext,
        ILogger<SearchSuggestHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the search suggest query.
    /// </summary>
    /// <param name="query">The search suggest query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of search suggestions.</returns>
    public async Task<List<SearchSuggestionDto>> Handle(SearchSuggestQuery query, CancellationToken cancellationToken)
    {
        // Return empty list if query is too short
        if (string.IsNullOrWhiteSpace(query.Q) || query.Q.Length < MinQueryLength)
        {
            this.logger.LogDebug("Search query too short or empty");
            return new List<SearchSuggestionDto>();
        }

        var limit = query.Limit.HasValue
            ? Math.Min(query.Limit.Value, MaxLimit)
            : DefaultLimit;

        if (limit <= 0)
        {
            limit = DefaultLimit;
        }

        var searchTerm = query.Q.ToLower();

        var truncatedQuery = query.Q.Substring(0, Math.Min(query.Q.Length, 50));
        var safeQueryForLog = truncatedQuery
            .Replace("\r", string.Empty)
            .Replace("\n", string.Empty);

        this.logger.LogInformation("Searching for suggestions with query: {Query}", safeQueryForLog);

        var suggestions = await this.dbContext.Products
            .Where(p => p.IsActive &&
                        (p.Name.ToLower().Contains(searchTerm) ||
                         p.Slug.ToLower().Contains(searchTerm)))
            .OrderBy(p => p.Name)
            .Take(limit)
            .Select(p => new SearchSuggestionDto(
                p.Id,
                p.Name,
                p.Slug,
                p.Images))
            .ToListAsync(cancellationToken);

        this.logger.LogInformation("Found {Count} suggestions", suggestions.Count);

        return suggestions;
    }
}
