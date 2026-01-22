// <copyright file="SearchSuggestQuery.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Search;

/// <summary>
/// Query to get search suggestions.
/// </summary>
/// <param name="Q">The search query.</param>
/// <param name="Limit">Maximum number of suggestions (default 10, max 20).</param>
public record SearchSuggestQuery(
    string? Q,
    int? Limit);
