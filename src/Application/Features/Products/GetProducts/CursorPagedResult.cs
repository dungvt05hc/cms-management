// <copyright file="CursorPagedResult.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Products.GetProducts;

/// <summary>
/// Cursor-based paginated result.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
/// <param name="Items">The items.</param>
/// <param name="NextCursor">Cursor for the next page (null if no more pages).</param>
/// <param name="HasMore">Whether there are more items to load.</param>
public record CursorPagedResult<T>(
    List<T> Items,
    string? NextCursor,
    bool HasMore);
