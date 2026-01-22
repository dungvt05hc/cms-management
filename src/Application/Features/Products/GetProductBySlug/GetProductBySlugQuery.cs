// <copyright file="GetProductBySlugQuery.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Products.GetProductBySlug;

/// <summary>
/// Query to get a product by its slug.
/// </summary>
/// <param name="Slug">The product slug.</param>
public record GetProductBySlugQuery(string Slug);
