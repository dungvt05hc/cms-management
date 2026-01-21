// <copyright file="DeleteProductCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Products.DeleteProduct;

/// <summary>
/// Command to delete a product.
/// </summary>
/// <param name="ProductId">The product identifier.</param>
public record DeleteProductCommand(Guid ProductId);
