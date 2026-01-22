// <copyright file="SearchSuggestionDto.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Search;

/// <summary>
/// Search suggestion data transfer object.
/// </summary>
/// <param name="Id">The product identifier.</param>
/// <param name="Name">The product name.</param>
/// <param name="Slug">The URL-friendly slug.</param>
/// <param name="Images">The images.</param>
public record SearchSuggestionDto(
    Guid Id,
    string Name,
    string Slug,
    string? Images);
