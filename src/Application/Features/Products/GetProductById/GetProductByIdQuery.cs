// <copyright file="GetProductByIdQuery.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Products.GetProductById;

/// <summary>
/// Query to get a product by ID.
/// </summary>
/// <param name="ProductId">The product identifier.</param>
public record GetProductByIdQuery(Guid ProductId);
