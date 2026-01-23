// <copyright file="UpdateProductGroupRequest.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Features.ProductGroups.CreateProductGroup;

namespace Api.Controllers;

/// <summary>
/// Request to update a product group.
/// </summary>
/// <param name="CategoryId">The category identifier.</param>
/// <param name="Name">The product group name.</param>
/// <param name="Attributes">The attribute definitions.</param>
public record UpdateProductGroupRequest(
    Guid CategoryId,
    string Name,
    List<ProductGroupAttributeDto> Attributes);
