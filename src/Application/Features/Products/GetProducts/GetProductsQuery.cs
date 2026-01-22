// <copyright file="GetProductsQuery.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Products.GetProducts;

/// <summary>
/// Query to get products with filters.
/// </summary>
/// <param name="CategoryId">Filter by category ID.</param>
/// <param name="CategorySlug">Filter by category slug.</param>
/// <param name="Q">Search term for name or slug.</param>
/// <param name="Featured">Filter by featured products.</param>
/// <param name="Sort">Sort order: priceAsc or priceDesc.</param>
/// <param name="Cursor">Cursor for pagination.</param>
/// <param name="Limit">Page size (default 20).</param>
/// <param name="Page">Page number (1-indexed) - for backward compatibility.</param>
/// <param name="PageSize">Page size - for backward compatibility.</param>
public record GetProductsQuery(
    Guid? CategoryId,
    string? CategorySlug,
    string? Q,
    bool? Featured,
    string? Sort,
    string? Cursor,
    int? Limit,
    int? Page,
    int? PageSize);
