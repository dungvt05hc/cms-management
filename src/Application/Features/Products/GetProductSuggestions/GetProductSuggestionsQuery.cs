// <copyright file="GetProductSuggestionsQuery.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Products.GetProductSuggestions;

/// <summary>
/// Query to get product suggestions (same category).
/// </summary>
/// <param name="Slug">The product slug.</param>
/// <param name="Limit">Maximum number of suggestions to return (default 4).</param>
public record GetProductSuggestionsQuery(string Slug, int Limit = 4);
