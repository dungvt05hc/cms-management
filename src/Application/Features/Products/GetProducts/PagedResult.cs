// <copyright file="PagedResult.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Products.GetProducts;

/// <summary>
/// Paginated result.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
/// <param name="Items">The items.</param>
/// <param name="TotalCount">Total count.</param>
/// <param name="Page">Current page.</param>
/// <param name="PageSize">Page size.</param>
public record PagedResult<T>(
    List<T> Items,
    int TotalCount,
    int Page,
    int PageSize)
{
    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)this.TotalCount / this.PageSize);
}
