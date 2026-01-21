// <copyright file="GetProductsQuery.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Products.GetProducts;

/// <summary>
/// Query to get products with filters.
/// </summary>
/// <param name="CategoryId">Filter by category.</param>
/// <param name="Q">Search term for name or slug.</param>
/// <param name="Featured">Filter by featured products.</param>
/// <param name="Page">Page number (1-indexed).</param>
/// <param name="PageSize">Page size.</param>
public record GetProductsQuery(
    Guid? CategoryId,
    string? Q,
    bool? Featured,
    int Page,
    int PageSize);
